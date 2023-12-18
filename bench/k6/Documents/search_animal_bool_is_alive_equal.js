import grpc from 'k6/net/grpc';
import {check} from 'k6';

const authorizationToken = 'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InJvb3QiLCJwZXJtaXNzaW9uIjoiNTExIiwibmJmIjoxNjk4OTI4OTA3LCJleHAiOjE3MDc0ODI1MDcsImlhdCI6MTY5ODkyODkwN30.G0QHZvrYw4wRN6zO2z5BsyhKn1sJCmGv5y3um53P5F53XmBcTwUZ8U94Lxy08eLZFLPLAZcLoxWqFgUAH4iQZg'

const data = {
    "PaginationDto": {
        "PageNumber": 0,
        "PageSize": 10
    },
    "SearchQueryDto": {
        "Boost": 1,
        "ExactBoolSearchQueryDto": {
            "FieldName": "IsAlive",
            "Value": true
        }
    },
    "TemplateName": "Animal"
};

const metadata = {
    'authorization': `bearer ${authorizationToken}`,
}

const client = new grpc.Client();
client.load([], '../../../proto/documents.proto');

export let options = {
    vus: 10,
    iterations: 100,
    insecureSkipTLSVerify: true,
    duration: '30m',
};

const cert = open('/home/sobczal/Devel/dotnet/Persistify/src/server/Persistify.Server/localhost.pfx');
export default () => {
    client.connect('localhost:5000', {
    });

    const response = client.invoke('/Persistify.Services.DocumentService/SearchDocuments', data, {metadata: metadata});

    check(response, {
        'status is OK': (r) => r && r.status === grpc.StatusOK,
    });

    client.close();
};
