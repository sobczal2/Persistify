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

    const token = 'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJTdXBlclVzZXIiLCJleHAiOjE2ODI5NTI1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMSJ9.7THPOixYh4wxPhBE4LvktEbPu29PCI25YmUTWuSJGLQIiTCea7HBKIBLRgn278rxw6Lp3s3XL0z6RpgMOUuYug'; // replace this with your actual JWT token

    const headers = {
        'authorization': `bearer ${token}`,
    };

    const response = client.invoke('/TypeService/List', data, { headers });

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
