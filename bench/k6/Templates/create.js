import grpc from 'k6/net/grpc';
import {check, sleep} from 'k6';

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
        "BoolFields": [
            {
                "IsRequired": false,
                "Name": "est id in ex"
            },
            {
                "IsRequired": true,
                "Name": "ea incididunt Duis eiusmod dolore"
            }
        ],
        "TextFields": [
            {
                "IsRequired": true,
                "Name": "test",
                "AnalyzerPresetName": "standard"
            },
            {
                "IsRequired": false,
                "Name": "test",
                "AnalyzerDescriptor": {
                    "CharacterFilterNames": [],
                    "TokenizerName": "standard",
                    "TokenFilterNames": [
                        "lowercase"
                    ]
                }
            }
        ],
        "NumberFields": [
            {
                "IsRequired": false,
                "Name": "test"
            },
        ],
        "TemplateName": makeid(10)
    };
    const response = client.invoke('/Persistify.Services.TemplateService/CreateTemplate', data);

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
