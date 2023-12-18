import grpc from 'k6/net/grpc';
import {check} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiNTExIiwibmJmIjoxNjk4OTI4OTA3LCJleHAiOjE3MDc0ODI1MDcsImlhdCI6MTY5ODkyODkwN30.G0QHZvrYw4wRN6zO2z5BsyhKn1sJCmGv5y3um53P5F53XmBcTwUZ8U94Lxy08eLZFLPLAZcLoxWqFgUAH4iQZg'

const data = {
    "FieldValueDtos": [
        {
            "TextFieldValueDto": {
                "Value": "Joe"
            },
            "FieldName": "Name"
        },
        {
            "TextFieldValueDto": {
                "Value": "duck"
            },
            "FieldName": "Species"
        },
        {
            "NumberFieldValueDto": {
                "Value": 10
            },
            "FieldName": "Weight"
        },
        {
            "DateTimeFieldValueDto": {
                "Value": {
                    "value": "110",
                    "scale": "HOURS",
                    "kind": "UTC"
                }
            },
            "FieldName": "BirthDate"
        },
        {
            "BoolFieldValueDto": {
                "Value": true
            },
            "FieldName": "IsAlive"
        },
        {
            "BinaryFieldValueDto": {
                "Value": 'a'.repeat(1000)
            },
            "FieldName": "Photo"
        }
    ],
    "TemplateName": "Animal"
};

const metadata = {
    'authorization': `bearer ${authorizationToken}`,
}

const client = new grpc.Client();
client.load([], '../../../proto/documents.proto');

export let options = {
    vus: 30,
    iterations: 1000000,
    insecureSkipTLSVerify: true,
    duration: '30m',
};

const cert = open('/home/sobczal/Devel/dotnet/Persistify/src/server/Persistify.Server/localhost.pfx');
export default () => {
    client.connect('localhost:5000', {
    });

    const response = client.invoke('/Persistify.Services.DocumentService/CreateDocument', data, {metadata: metadata});

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
