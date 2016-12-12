//
//  test_tensorflow.h
//  Unity-iPhone
//
//  Created by Zeratul Phillip on 11/18/16.
//
//

#ifndef test_tensorflow_h
#define test_tensorflow_h

#include <fstream>

#include "tensorflow/core/public/session.h"


tensorflow::Status LoadModel(NSString* fileName, NSString* fileType,
                             std::unique_ptr<tensorflow::Session>* session);


#endif /* test_tensorflow_h */
