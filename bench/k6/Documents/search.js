import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const client = new grpc.Client();
client.load([], '../../../proto/documents.proto');
export default () => {
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "Pagination": {
            "PageNumber": 0,
            "PageSize": 10
        },
        "SearchNode": {
            "NumberRangeSearchNode": {
                "FieldName": "NumberOfLegs",
                "Min": 0,
                "Max": 100,
                "IncludeMax": true,
                "IncludeMin":false
            }
        },
        "TemplateId": 1
    };
    const response = client.invoke('/Persistify.Services.DocumentService/SearchDocuments', data);

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
