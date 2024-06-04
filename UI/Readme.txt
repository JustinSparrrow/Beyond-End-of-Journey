代码需要完成的功能——
主菜单【Menu】：
在【MainMenu】中：
	按钮
	【开始游戏】：点击跳转到第一个场景【新手村】
       	【选项】：无
     	【退出】：退出程序

在【Inventory】-【prefab】中/【Bag】场景中：
背包【BagUI】：
	按钮
	需要实现按tab弹出背包
	【prop】模板：导入道具图片，number是道具数量
	实现：点击Image后，Explanation文本框中显示道具用途（实现不了则删除Explanation）
	道具的存储
	【使用】：选中道具后，点击使用（留着好看也行）
	点击x标关闭背包

在【Dialogue】-【UI】中
【Dialogue】模板：
	需要实现与人物触发后按E键进行对话交互。
	【Character】：放角色图片；
	【Name】：角色名字；
	【Text】：对话文本
	对话结束后对话框消失。
【Task】模块：发布任务
	【Text】：任务内容
	点x标关闭任务ui。

在【Skill】场景中
【Panel】：
	按钮【skill】：导入技能素材，点击Image后explanation中显示技能的相关信息。
	需要实现：技能导入、技能点击后使用、技能禁止使用
	