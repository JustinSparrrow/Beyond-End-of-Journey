# Github仓库使用方法
——————————————————————————————————————————————————————————————

### 这次主要用到两个教程
git的安装教程：https://www.cnblogs.com/kevinzhushrek/p/16092144.html

gitee仓库的教程：https://cloud.tencent.com/developer/article/1862274

git的安装步骤不用改变所以不再复述

连接到github仓库的命令和gitee的略有不同，所以把所有指令都放在这里
```
git init
git add .
git commit -m "注释说明"
git remote add origin https://github.com/JustinSparrrow/Beyond-End-of-Journey.git
git pull --rebase origin main
git push -u origin master
```
下次使用时要先拉取再上传
```
git add .
git commit -m "注释说明"
git pull --rebase origin main
git push -u origin master
```