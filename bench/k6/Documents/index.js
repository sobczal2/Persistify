import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const client = new grpc.Client();
client.load([], '../../../proto/documents.proto');
export default () => {
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "BoolFieldValues": [
            {
                "FieldName": "occaecat",
                "Value": true
            },
            {
                "FieldName": "Lorem amet sed consectetur",
                "Value": false
            },
            {
                "Value": true,
                "FieldName": "aliqua id"
            },
            {
                "Value": false,
                "FieldName": "ut Excepteur tempor"
            }
        ],
        "NumberFieldValues": [
            {
                "FieldName": "aliquip in cupidatat nostrud",
                "Value": 86718206.21563268
            },
            {
                "Value": "-Infinity",
                "FieldName": "ut in pariatur officia"
            },
            {
                "FieldName": "eiusmod",
                "Value": "-Infinity"
            },
            {
                "Value": "NaN",
                "FieldName": "in"
            },
            {
                "FieldName": "est minim elit",
                "Value": "-Infinity"
            }
        ],
        "TextFieldValues": [
            {
                "Value": "eu non",
                "FieldName": "nostrud labore"
            },
            {
                "FieldName": "deserunt veniam laboris ut",
                "Value": "incididunt aute adipisicing"
            },
            {
                "FieldName": "in deserunt aute",
                "Value": "elit occaecat ullamco ad enim"
            },
            {
                "FieldName": "sit occaecat ad",
                "Value": "eu tempor"
            },
            {
                "FieldName": "deserunt",
                "Value": "culpa"
            }
        ],
        "TemplateId": 1
    };
    const response = client.invoke('/Persistify.Services.DocumentService/IndexDocument', data);

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
