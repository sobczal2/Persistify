import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiNTExIiwibmJmIjoxNzAyMzgyMzEyLCJleHAiOjE3MDQ5NzQzMTIsImlhdCI6MTcwMjM4MjMxMn0.DnqFpL_W5cyBLbOK3-MoOvinSRw7_kj20qxlUUQ1ZkEHlQPG-u9CcNnQIDnGIAs99ig67x_km1kwc0QKqnSIBw'

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
                "Name": "Name",
                "Required": true,
                "TextFieldDto": {
                    "AnalyzerDto": {
                        "PresetNameAnalyzerDto": {
                            "PresetName": "standard"
                        }
                    },
                    "IndexText": true,
                    "IndexFullText": true
                }
            },
            {
                "Name": "Species",
                "Required": true,
                "TextFieldDto": {
                    "AnalyzerDto": {
                        "PresetNameAnalyzerDto": {
                            "PresetName": "standard"
                        }
                    },
                    "IndexText": true,
                    "IndexFullText": true
                }
            },
            {
                "Name": "Weight",
                "Required": true,
                "NumberFieldDto": {
                    "Index": true
                }
            },
            {
                "Name": "BirthDate",
                "Required": true,
                "DateTimeFieldDto": {
                    "Index": true
                }
            },
            {
                "Name": "IsAlive",
                "Required": true,
                "BoolFieldDto": {
                    "Index": true
                }
            },
            {
                "Name": "Photo",
                "Required": true,
                "BinaryFieldDto": {}
            }
        ],
        "TemplateName": "Animal"
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
