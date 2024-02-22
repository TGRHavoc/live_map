const regex = /^version \"(.+)\"$/gm;

module.exports.readVersion = function(contents) {
    let version = regex.exec(contents);
    return version[1];
}

module.exports.writeVersion = function(contents, version){
    let replaced = contents.replace(regex, `version "${version}"`);    
    return replaced;
}