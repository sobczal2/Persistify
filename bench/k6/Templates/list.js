import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const client = new grpc.Client();
client.load([], '../../../proto/templates.proto');

export default () => {
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "Pagination": {
            "PageNumber": 0,
            "PageSize": 10
        }
    };

    const response = client.invoke('/Persistify.Services.TemplateService/ListTemplates', data);

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
