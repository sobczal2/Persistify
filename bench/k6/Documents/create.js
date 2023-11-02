import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiNTExIiwibmJmIjoxNjk4OTI4OTA3LCJleHAiOjE3MDc0ODI1MDcsImlhdCI6MTY5ODkyODkwN30.G0QHZvrYw4wRN6zO2z5BsyhKn1sJCmGv5y3um53P5F53XmBcTwUZ8U94Lxy08eLZFLPLAZcLoxWqFgUAH4iQZg'

const client = new grpc.Client();
client.load([], '../../../proto/documents.proto');
export default () => {
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "FieldValueDtos": [
            {
                "TextFieldValueDto": {
                    "Value": "duck"
                },
                "FieldName": "Name"
            },
            {
                "NumberFieldValueDto": {
                    "Value": 10
                },
                "FieldName": "Age"
            },
            {
                "BoolFieldValueDto": {
                    "Value": true
                },
                "FieldName": "IsCute"
            },
            {
                "DateTimeFieldValueDto": {
                    "Value": {
                        "value": "0",
                        "scale": "HOURS",
                        "kind": "UTC"
                    }
                },
                "FieldName": "CreatedAt"
            },
            {
                "BinaryFieldValueDto": {
                    "Value": ""
                },
                "FieldName": "Photo"
            }
        ],
        "TemplateName": "Animal"
    };

    const metadata = {
        'authorization': `bearer ${authorizationToken}`,
    }

    const response = client.invoke('/Persistify.Services.DocumentService/CreateDocument', data, {metadata: metadata});

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
