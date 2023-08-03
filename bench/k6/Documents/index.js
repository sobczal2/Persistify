import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const client = new grpc.Client();
client.load([], '../../../proto/documents.proto');
export default () => {
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "TextFieldValues": [
            {
                "FieldName": "Name",
                "Value": "Duck"
            }
        ],
        "NumberFieldValues": [
            {
                "FieldName": "NumberOfLegs",
                "Value": Math.floor(Math.random() * 100) + 1
            }
        ],
        "BoolFieldValues": [
            {
                "FieldName": "IsFriendly",
                "Value": false
            }
        ],
        "TemplateId": 0
    };
    const response = client.invoke('/Persistify.Services.DocumentService/AddDocument', data);

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
