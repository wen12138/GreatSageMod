# GreatSageMod

## Mod效果

- 在普通棍法时,再次按下当前棍法按键即可进入大圣模式(默认只可以在立棍法进入)
- 在大圣模式时,按下任意普通棍法按键即可切换回普通棍法。
- 无限时长,任意地图,无需根器,无需大圣五件套,无需五段棍启动,不受大圣残躯二阶段影响
- 可以通过修改配置文件 "GreatSageConfig.json" 调整可以从哪些普通棍法进入大圣模式

## 配置文件介绍
- EnterGreatSageModeFromSmashStance 是否可以在劈棍法时再次按下劈棍法键进入大圣模式
- EnterGreatSageModeFromPillarStance ~立棍法时~
- EnterGreatSageModeFromThrustStance ~戳棍法时~
- true 为可以，false 为不可以
- 修改配置文件后需要重新启动游戏才可以生效

> [!IMPORTANT]
> 
>  注意: 需要黑神话:悟空游戏文件夹内已有CSharpLoader with hook才可使用该Mod

## 项目使用和Mod使用

- 使用Visual Studio或Rider项目直接编译即可在编译输出目录中得到GreatSageMod.dll
- 将该dll文件按以下相对路径放在黑神话:悟空游戏目录中CSharLoader的Mods文件夹内即可被CSharpLoader识别并加载
- 目录: b1\Binaries\Win64\CSharpLoader\Mods\GreatSageMod\GreatSageMod.dll

> [!NOTE]
> 
> [可以自己调整该项目的编译后处理事件，将批处理命令的目标文件夹设置为自己电脑的上述目录，可以在开发中省略手动复制的步骤]


---

## Mod Effects

- In normal stance, press current stance button again will enter Great Sage mode(Only can enter in pillar stance at default).
- In Great Sage mode, press any normal stance button will switch back to normal stance.
- No time limit. No map limit. No relic limit. No equipment limit. Don't Need five focus points. Don't be affected by the second stage of the Great Sage's Broken Body.
- You can adjust which stance can enter Great Sage Mode by modifying the config file "GreatSageConfig.json".

## Config File
- EnterGreatSageModeFromSmashStance Can enter Great Sage mode in smash stance
- EnterGreatSageModeFromPillarStance ~ in pillar stance
- EnterGreatSageModeFromThrustStance  ~ in thrust stance
- true is yes, false is no
- To apply the modification of the config file, you should restart the game application.

> [!IMPORTANT]
> 
> Attention: This mod dependent on CSharpLoader with hook, you should download it in advance.

## How to use the project and mod
Complie the project by using Visual Studio or Rider, and you can get "GreatSageMod.dll" in output path.
Copy "GreatSageMod.dll" to "CSharpLoader/Mods" in the following relative path of Black Myth: Wukong game file.
- Path: b1\Binaries\Win64\CSharpLoader\Mods\GreatSageMod\GreatSageMod.dll

> [!NOTE]
>
> You can adjust the post-compile processing events of the project yourself, set the target folder of batch commands to the directory on your computer, and you can omit the manual copy step in development

 ## CSharpLoader

- Github仓库地址/Github Repositry:<https://github.com/czastack/B1CSharpLoader>
- N网地址/Nexus Site:<https://www.nexusmods.com/blackmythwukong/mods/664>
