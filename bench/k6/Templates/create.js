import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiMTI3IiwibmJmIjoxNjk2OTMyMDE0LCJleHAiOjE2OTk1MjQwMTQsImlhdCI6MTY5NjkzMjAxNH0.nYJc6f1BkY0VlEmS4vgDyDzBctmbI3u8VWMz4uf4urytul-5PtaxKrSOhD4WEIUO3xdKdOsu92ley6vfNOGwng'

const client = new grpc.Client();
client.load([], '../../../proto/templates.proto');

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
    client.connect('localhost:5000', {
        plaintext: true,
    });

    const data = {
        "TextFields": [
            {
                "Name": "Name",
                "Required": false,
                "TextField": {
                    "AnalyzerDescriptor": {
                        "PresetAnalyzerDescriptor": {
                            "PresetName": "standard"
                        }
                    }
                }
            }
        ],
        "NumberFields": [
            {
                "Name": "Age",
                "Required": true,
                "NumberField": {}
            }
        ],
        "BoolFields": [
            {
                "Name": "IsCute",
                "Required": true,
                "BoolField": {}
            }
        ],
        "TemplateName": makeid(10)
    };

    const metadata = {
        'authorization': `bearer ${authorizationToken}`,
    }

    const response = client.invoke('/Persistify.Services.TemplateService/CreateTemplate', data, {metadata: metadata});

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
