import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiNTExIiwibmJmIjoxNjk4ODg3MDcwLCJleHAiOjE3MDc0NDA2NzAsImlhdCI6MTY5ODg4NzA3MH0.d3UF-W1sBiZQP6Qg5sfumX2Xjp9YvDTQkpWFTH1S0rBMAPq0GyErzLb3KHEZf1laudcRRl3iVNUIH9AIlP6Lrg'

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
        "Fields": [
            {
                "TextFieldDto": {
                    "AnalyzerDto": {
                        "PresetNameAnalyzerDto": {
                            "PresetName": "standard"
                        }
                    }
                },
                "Name": "Name",
                "Required": false
            },
            {
                "Name": "Age",
                "Required": true,
                "NumberFieldDto": {}
            },
            {
                "Name": "IsCute",
                "Required": true,
                "BoolFieldDto": {}
            },
            {
                "Name": "CreatedAt",
                "Required": true,
                "DateTimeFieldDto": {}
            },
            {
                "Name": "Photo",
                "Required": true,
                "BinaryFieldDto": {}
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
