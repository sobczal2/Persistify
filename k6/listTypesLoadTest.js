import grpc from 'k6/net/grpc';
import { check, sleep } from 'k6';

const client = new grpc.Client();
client.load(['definitions'], 'shared.proto');
client.load(['definitions'], 'types.proto');
client.load(['definitions'], 'services.proto');

function makeid(length) {
    let result = '';
    const characters = 'abcdefghijklmnopqrstuvwxyz';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
}

export default () => {
    client.connect('localhost:5001', {
        plaintext: true,
    });

    const data = {
        "PaginationRequest": {
            "PageNumber": 1,
            "PageSize": 100
        }
    };
    const response = client.invoke('/TypesService/List', data);

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
