# Changelog

### [2.3.4](https://github.com/TGRHavoc/live_map/compare/v2.3.2...v2.3.4) (2019-07-11)


### Bug Fixes

* blip file errors ([6e85c30](https://github.com/TGRHavoc/live_map/commit/6e85c30))
* directory not being created for blips file ([907c4ea](https://github.com/TGRHavoc/live_map/commit/907c4ea))


### Changes

* add comma to entries in blip_helper ([db39357](https://github.com/TGRHavoc/live_map/commit/db39357)), closes [#32](https://github.com/TGRHavoc/live_map/issues/32)



### [2.3.3](https://github.com/TGRHavoc/live_map/compare/v2.3.2...v2.3.3) (2019-07-11)


### Bug Fixes

* directory not being created for blips file ([907c4ea](https://github.com/TGRHavoc/live_map/commit/907c4ea))


### Changes

* add comma to entries in blip_helper ([db39357](https://github.com/TGRHavoc/live_map/commit/db39357)), closes [#32](https://github.com/TGRHavoc/live_map/issues/32)



### [2.3.2](https://github.com/TGRHavoc/live_map/compare/v2.3.1...v2.3.2) (2019-06-11)


### Bug Fixes

* **sockets:** removeplayer function `paylod` -> `payload` ([0a5de15](https://github.com/TGRHavoc/live_map/commit/0a5de15))


### Changes

* **sockets:** conform to own style ([ead5fc2](https://github.com/TGRHavoc/live_map/commit/ead5fc2))


### [2.3.1](https://github.com/TGRHavoc/live_map/compare/v2.3.0...v2.3.1) (2019-06-10)


### Bug Fixes

* players not being removed after leaving server ([81811fb](https://github.com/TGRHavoc/live_map/commit/81811fb))
* webpack and other dependencies not loading ([e1de1d6](https://github.com/TGRHavoc/live_map/commit/e1de1d6))



## [2.3.0](https://github.com/TGRHavoc/live_map/compare/v2.2.12...v2.3.0) (2019-06-09)


### Bug Fixes

* events wrapper not being used ([c3f31ae](https://github.com/TGRHavoc/live_map/commit/c3f31ae))
* **blips:** addblip incorrectly reporting duplicate blips ([b70b345](https://github.com/TGRHavoc/live_map/commit/b70b345))


### Changes

* change how blip controller is created ([2f4997f](https://github.com/TGRHavoc/live_map/commit/2f4997f))
* remove C# files ([ba6ef5a](https://github.com/TGRHavoc/live_map/commit/ba6ef5a))
* update server to use new controllers ([793512d](https://github.com/TGRHavoc/live_map/commit/793512d))
* update socket controller ([57a6094](https://github.com/TGRHavoc/live_map/commit/57a6094))


### Features

* add basic blip controller ([64ff008](https://github.com/TGRHavoc/live_map/commit/64ff008))
* add basic server for node ([7892791](https://github.com/TGRHavoc/live_map/commit/7892791))
* add basic websocket server ([5bbee05](https://github.com/TGRHavoc/live_map/commit/5bbee05))
* add webpack ([6b0e711](https://github.com/TGRHavoc/live_map/commit/6b0e711))
* fully implement socketcontroller ([9a206ef](https://github.com/TGRHavoc/live_map/commit/9a206ef))
* move blip_server to node ([744ac41](https://github.com/TGRHavoc/live_map/commit/744ac41))
* move wrapper to node ([e67776f](https://github.com/TGRHavoc/live_map/commit/e67776f))


### BREAKING CHANGES

* remove C# files



## [2.2.12](https://github.com/TGRHavoc/live_map/compare/v2.2.0...v2.2.12) 


### Changes

* add license to reverse_location_hashes ([6625ce2](https://github.com/TGRHavoc/live_map/commit/6625ce2))
* add license to update_check ([30c84fa](https://github.com/TGRHavoc/live_map/commit/30c84fa)) 
* remove console spam ([701ca08](https://github.com/TGRHavoc/live_map/commit/701ca08))
* remove location data ([67ad3b6](https://github.com/TGRHavoc/live_map/commit/67ad3b6))
* update client.lua to use new reverse function ([81c6465](https://github.com/TGRHavoc/live_map/commit/81c6465))
* update the timer in update_check ([c9fae47](https://github.com/TGRHavoc/live_map/commit/c9fae47))


### Features

* add location display and more ([cb5be6f](https://github.com/TGRHavoc/live_map/commit/cb5be6f))
* add location to player data ([db610a7](https://github.com/TGRHavoc/live_map/commit/db610a7))
* add plate check to client.lua ([4b783ff](https://github.com/TGRHavoc/live_map/commit/4b783ff))
* add reverse_car_hashes ([4774170](https://github.com/TGRHavoc/live_map/commit/4774170))



## [2.2.0](https://github.com/TGRHavoc/live_map/compare/v2.1.8...v2.2.0) 


### Bug Fixes

* add null checks to various API functions ([bbfc23b](https://github.com/TGRHavoc/live_map/commit/bbfc23b)), closes [#24](https://github.com/TGRHavoc/live_map/issues/24)
* player location not being sent to sockets ([0974a65](https://github.com/TGRHavoc/live_map/commit/0974a65)), closes [#21](https://github.com/TGRHavoc/live_map/issues/21)
* too many files open error ([345e266](https://github.com/TGRHavoc/live_map/commit/345e266))
* typo in client.lua and SocketHandler ([461e92c](https://github.com/TGRHavoc/live_map/commit/461e92c))



## [2.1.8](https://github.com/TGRHavoc/live_map/compare/v2.1.7...v2.1.8) 


### Bug Fixes

* async issues ([f51e974](https://github.com/TGRHavoc/live_map/commit/f51e974))
* async write errors ([64583fe](https://github.com/TGRHavoc/live_map/commit/64583fe))
* Server_OnError error ([265b3b6](https://github.com/TGRHavoc/live_map/commit/265b3b6))


### Features

* add event handlers for blips ([acc71d4](https://github.com/TGRHavoc/live_map/commit/acc71d4))



## [2.1.7](https://github.com/TGRHavoc/live_map/compare/v2.1.6...v2.1.7) 


### Bug Fixes

* race condition and null data ([5a37bfc](https://github.com/TGRHavoc/live_map/commit/5a37bfc))


### Changes

* add error handling to update_check ([9f22b21](https://github.com/TGRHavoc/live_map/commit/9f22b21))



## [2.1.6](https://github.com/TGRHavoc/live_map/compare/v2.1.4...v2.1.6) 


### Bug Fixes

* clients crashing when player leaves ([ba7aa6f](https://github.com/TGRHavoc/live_map/commit/ba7aa6f))


### Changes

* create test.lua ([46748a3](https://github.com/TGRHavoc/live_map/commit/46748a3))
* delete test.lua ([bfc3a76](https://github.com/TGRHavoc/live_map/commit/bfc3a76))
* remove spammy prints ([603e370](https://github.com/TGRHavoc/live_map/commit/603e370))



## [2.1.4](https://github.com/TGRHavoc/live_map/compare/v2.1.3...v2.1.4) 


### Bug Fixes

* sending client data when websocket disconnects ([f12837f](https://github.com/TGRHavoc/live_map/commit/f12837f))


### Changes

* remove spammy traces ([f5e856e](https://github.com/TGRHavoc/live_map/commit/f5e856e))


### Features

* add update_check ([c592948](https://github.com/TGRHavoc/live_map/commit/c592948))



## [2.1.3](https://github.com/TGRHavoc/live_map/compare/v2.1.2...v2.1.3) 


### Bug Fixes

* listener only listening on loopback address ([2cb24eb](https://github.com/TGRHavoc/live_map/commit/2cb24eb))


### Changes

* update server comments ([d687c5f](https://github.com/TGRHavoc/live_map/commit/d687c5f))



## [2.1.2](https://github.com/TGRHavoc/live_map/compare/v2.1.1...v2.1.2) 


### Changes

* update newtonsoft package ([8fc3cf4](https://github.com/TGRHavoc/live_map/commit/8fc3cf4))
* update socketHandler ([3287ebe](https://github.com/TGRHavoc/live_map/commit/3287ebe)), closes [#6](https://github.com/TGRHavoc/live_map/issues/6)



## [2.1.1](https://github.com/TGRHavoc/live_map/compare/v2.1.0...v2.1.1) 


### Changes

* update how players are handled ([5fbd071](https://github.com/TGRHavoc/live_map/commit/5fbd071))



## [2.1.0](https://github.com/TGRHavoc/live_map/compare/v2.0.0...v2.1.0) 


### Bug Fixes

* events not being registered ([963711b](https://github.com/TGRHavoc/live_map/commit/963711b))


### Changes

* remove old files ([5a99b20](https://github.com/TGRHavoc/live_map/commit/5a99b20))
* slighly better logging ([5940163](https://github.com/TGRHavoc/live_map/commit/5940163))
* update gitignore ([054eeee](https://github.com/TGRHavoc/live_map/commit/054eeee))


### Features

* add ability to remove players and data ([37c03b3](https://github.com/TGRHavoc/live_map/commit/37c03b3))
* add allow-origin header ([b1bb783](https://github.com/TGRHavoc/live_map/commit/b1bb783))
* add blip generation with command ([afb9996](https://github.com/TGRHavoc/live_map/commit/afb9996)), closes [#3](https://github.com/TGRHavoc/live_map/issues/3)
* add blip helper ([dde519d](https://github.com/TGRHavoc/live_map/commit/dde519d)), closes [#2](https://github.com/TGRHavoc/live_map/issues/2) [#2](https://github.com/TGRHavoc/live_map/issues/2)
* add blips.json file ([d250dda](https://github.com/TGRHavoc/live_map/commit/d250dda)), closes [#5](https://github.com/TGRHavoc/live_map/issues/5)
* add default client file ([8dca6b3](https://github.com/TGRHavoc/live_map/commit/8dca6b3))
* add reverse weapon hash file ([58d9bb9](https://github.com/TGRHavoc/live_map/commit/58d9bb9))
* add vehicle icons ([69208fb](https://github.com/TGRHavoc/live_map/commit/69208fb))
* update various blip mechanics ([5b5a193](https://github.com/TGRHavoc/live_map/commit/5b5a193))



## [2.0.0](https://github.com/TGRHavoc/live_map/compare/v1.0.0...v2.0.0) 


### Changes

* add resource_manifest_version ([912f3a1](https://github.com/TGRHavoc/live_map/commit/912f3a1))
* remove file writer and console.writelines ([1ee0619](https://github.com/TGRHavoc/live_map/commit/1ee0619))
* remove license ([4cc1b00](https://github.com/TGRHavoc/live_map/commit/4cc1b00))
* update comments ([95cae9b](https://github.com/TGRHavoc/live_map/commit/95cae9b))
* update O'Neil Ranch icon ([d4613e6](https://github.com/TGRHavoc/live_map/commit/d4613e6))
* update resource_manifest_version to the latest(?) one ([9de1ccd](https://github.com/TGRHavoc/live_map/commit/9de1ccd))
* update websocket handler ([9cda66b](https://github.com/TGRHavoc/live_map/commit/9cda66b))


### Features

* ability to add custom data to players ([60af4ea](https://github.com/TGRHavoc/live_map/commit/60af4ea))
* add blip helper ([0e22f14](https://github.com/TGRHavoc/live_map/commit/0e22f14))
* add gas station blips ([ce6f4eb](https://github.com/TGRHavoc/live_map/commit/ce6f4eb))
* add lua files ([11a5e5f](https://github.com/TGRHavoc/live_map/commit/11a5e5f))
* add utility events ([cfe1843](https://github.com/TGRHavoc/live_map/commit/cfe1843))
* add vehicle data to player ([e262ac8](https://github.com/TGRHavoc/live_map/commit/e262ac8))
* fx server compatability ([fee9958](https://github.com/TGRHavoc/live_map/commit/fee9958)), closes [#4](https://github.com/TGRHavoc/live_map/issues/4) [#1](https://github.com/TGRHavoc/live_map/issues/1)
* ssl support ([4358ec9](https://github.com/TGRHavoc/live_map/commit/4358ec9))
