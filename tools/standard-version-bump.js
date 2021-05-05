
module.exports.readVersion = function (contents) {
    return JSON.parse(contents).resource;
};

module.exports.writeVersion = function (contents, version) {
    const json = JSON.parse(contents);
    json.resource = version;
    return JSON.stringify(json, null, 4);
};