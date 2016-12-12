//
//  test_tensorflow.m
//  Unity-iPhone
//
//  Created by Zeratul Phillip on 11/18/16.
//
//

#include "test_tensorflow.h"


static NSString* model_file_name = @"opt_mannual_atrous"; // opt_conv2d_model // opt_mannual_atrous
static NSString* model_file_type = @"pb";
static NSString* model_file_subfolder = @"Models";

static const std::string inputLayerName  = "NETWORK_INPUT";       // NETWORK_INPUT
static const std::string outputLayerName = "BiasAdd_9";        // Conv2D_9 // transpose_31

//--------------------- Function Definition ---------------------------------
class IfstreamInputStream : public ::google::protobuf::io::CopyingInputStream
{
public:
    explicit IfstreamInputStream(const std::string& fileName)
    : ifs_(fileName.c_str(), std::ios::in | std::ios::binary) {}
    ~IfstreamInputStream() { ifs_.close(); }
    
    int Read(void* buffer, int size)
    {
        if (!ifs_)
            return -1;
        ifs_.read(static_cast<char*>(buffer), size);
        return (int)ifs_.gcount();
    }
    
private:
    std::ifstream ifs_;
};

tensorflow::Status loadModel(NSString* fileName, NSString* fileType, NSString* subfolder,
                             std::unique_ptr<tensorflow::Session>* session);

NSString* filePathForResourceName(NSString* name, NSString* extension, NSString* subfolder);

bool portableReadFileToProto(const std::string& fileName,
                             ::google::protobuf::MessageLite* proto);


//--------------------- Function Implimentation -----------------------------
//--------------------- test_TensorFlowLoadModel ----------------------------
tensorflow::Status loadModel(NSString* fileName, NSString* fileType, NSString* subfolder,
                             std::unique_ptr<tensorflow::Session>* session)
{
    tensorflow::SessionOptions options;
    tensorflow::Session* sessionPointer = nullptr;
    tensorflow::Status sessionStatus =
        tensorflow::NewSession(options, &sessionPointer);

    if (!sessionStatus.ok())
    {
        LOG(ERROR) << "test_tensorflow.mm : Could not create TensorFlow Session : "
                   << sessionStatus;
        return sessionStatus;
    }
    session->reset(sessionPointer);
    
    tensorflow::GraphDef tensorflowGraph;
    
    NSString* modelPath = filePathForResourceName(fileName, fileType, subfolder);
    if (!modelPath)
    {
        LOG(ERROR) << "test_tensorflow.mm : Failed to find model proto at "
                   << [fileName UTF8String] << [fileType UTF8String];
        return tensorflow::errors::NotFound([fileName UTF8String],
                                            [fileType UTF8String]);
    }
    const bool readProtoSucceeded =
        portableReadFileToProto([modelPath UTF8String], &tensorflowGraph);
    if (!readProtoSucceeded)
    {
        LOG(ERROR) << "test_tensorflow.mm : Failed to load model proto from"
                   << [modelPath UTF8String];
        return tensorflow::errors::NotFound([modelPath UTF8String]);
    }
    
    tensorflow::Status createStatus = (*session)->Create(tensorflowGraph);
    if (!createStatus.ok())
    {
        LOG(ERROR) << "test_tensorflow.mm : Could not create TensorFlow Graph: "
                   << createStatus;
        return createStatus;
    }
    
    return tensorflow::Status::OK();
}


NSString* filePathForResourceName(NSString* name, NSString* extension, NSString* subfolder)
{
    NSString* filePath = [[NSBundle mainBundle]
                          pathForResource:name
                          ofType:extension
                          inDirectory:subfolder];
    if (filePath == NULL)
    {
        LOG(FATAL) << "test_tensorflow.mm : Couldn't find '"
                   << [name UTF8String] << "."
                   << [extension UTF8String] << "' in bundle.";
        return nullptr;
    }
    return filePath;
}


bool portableReadFileToProto(const std::string& fileName,
                             ::google::protobuf::MessageLite* proto)
{
    ::google::protobuf::io::CopyingInputStreamAdaptor stream(
        new IfstreamInputStream(fileName));
    stream.SetOwnsCopyingStream(true);
    ::google::protobuf::io::CodedInputStream codedStream(&stream);
    // Total bytes hard limit set to 1GB
    // Warning limit set 512MB
    codedStream.SetTotalBytesLimit(1024LL << 20, 512LL << 20);
    return proto->ParseFromCodedStream(&codedStream);
}


//--------------------- test_SendArray --------------------------------------
extern "C"{
float* dll_SendArray(void* array, int channel, int width, int height, int klass)
{
    LOG(INFO) << "test_tensorflow.mm : in function dll_SendArray.";
    float* sourceData = (float* )array;

    // Cast double array[] to TensorFlow Tensor
    tensorflow::Tensor inputTensor(tensorflow::DT_FLOAT,
                                   tensorflow::TensorShape({1, height, width, channel}));
    
    auto inputTensorMapped = inputTensor.tensor<float, 4>();

    for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
            for (int k = 0; k < channel; k++)
                inputTensorMapped(0, i, j, k) = *(sourceData + i*width*channel + j*channel + k);
    
    // Load Model
    std::unique_ptr<tensorflow::Session> tensorFlowSession;
    tensorflow::Status loadStatus =
        loadModel(model_file_name, model_file_type, model_file_subfolder, &tensorFlowSession);
    
    if (!loadStatus.ok())
        LOG(FATAL) << "test_tensorflow.mm : Couldn't load model: " << loadStatus;
    else
        LOG(INFO) << "test_tensorflow.mm : load model Success!";
    
    // Run tensorflow
    float *outputArray;
    outputArray = new float[width*height*klass];
    
    if (tensorFlowSession.get()) {
        std::vector<tensorflow::Tensor> outputTensor;
        LOG(INFO) << "flag 1";
        tensorflow::Status run_status = tensorFlowSession->Run(
            {{inputLayerName, inputTensor}}, {outputLayerName}, {}, &outputTensor);
        LOG(INFO) << "flag 2, outputTensor.size() = " << outputTensor.size();
        if (!run_status.ok()) {
            LOG(ERROR) << "Running model failed:" << run_status;
        } else {
            tensorflow::Tensor *output = &outputTensor[0];
            // predictions : Eigen::Tensor of size : channels*width*height
            auto predictions = output->flat<float>();
            
            LOG(INFO) << "test_tensorflow.mm : prediction.size() = " << predictions.size();
            
            /*for (int i = 0; i < 18; i++)
            {
                float predictionValue = predictions(i);
                LOG(INFO) << "predictions[" << i << "] = " << predictionValue;
            }*/
            
            //float biases[6] = {0.14540546, -0.05354437, -0.01566526, -0.17373973, -0.05034233, -0.22464657};
            
            for (int i = 0; i < predictions.size(); i++)
            {
                float predictionValue = predictions(i);
                outputArray[i] = predictionValue;// + biases[i%6];
            }
        }
    }
    return outputArray;
}

    
int dll_ReleaseMemory(int* pArray)
{
    delete[] pArray;
    return 0;
}
}
























