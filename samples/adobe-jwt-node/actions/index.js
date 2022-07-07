const auth=require("@adobe/jwt-auth");
const _config=require("./config.js");

async function main(params) {

var retbody = {};
var accesstoken = await auth(params);

return {
        statusCode: 200,
        body: accesstoken
    };
}
exports.main = main;

