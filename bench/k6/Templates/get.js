import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiMTI3IiwibmJmIjoxNjk2OTMyMDE0LCJleHAiOjE2OTk1MjQwMTQsImlhdCI6MTY5NjkzMjAxNH0.nYJc6f1BkY0VlEmS4vgDyDzBctmbI3u8VWMz4uf4urytul-5PtaxKrSOhD4WEIUO3xdKdOsu92ley6vfNOGwng'

const client = new grpc.Client();
client.load([], '../../../proto/templates.proto');

export default () => {
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "TemplateName": "Animal"
    };

    const metadata = {
        'authorization': `bearer ${authorizationToken}`,
    }

    const response = client.invoke('/Persistify.Services.TemplateService/GetTemplate', data, {metadata: metadata});

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
