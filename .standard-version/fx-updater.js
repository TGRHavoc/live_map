const regex = new RegExp("^version \"(.+)\"");

module.exports.readVersion = function(contents) {
    // FUCK JAVASCRIPT.
    /**
    The original method was just `return regex.exec(contents)[1];
    but, apparently that fails... Because of the "start of line" in the regex
    So... I've got to split the fucking file and check against the regex that way
     */
    let c = contents.split("\n");
    let version = "Unknown";
    c.forEach(ele => {
        if (regex.test(ele)){
            version = regex.exec(ele)[1];
        }
    });
    return version;
}

module.exports.writeVersion = function(contents, version){
    let replaced = contents.replace(regex, `version "${version}"`);
    
    return replaced;
}
