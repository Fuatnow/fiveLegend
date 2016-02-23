using UnityEngine;
using System.Collections;
using System;

using cn.bmob.io;
public class BmobGameObject : BmobTable
{
	//score、playerName、cheatMode是后台数据表对应的字段名称
	public BmobInt score { get; set; }
	public String playerName { get; set; }
	public BmobBoolean cheatMode { get; set; }
	
	//读字段值
	public override void readFields(BmobInput input)
	{
		base.readFields(input);
		
		this.score = input.getInt("score");
		this.cheatMode = input.getBoolean("cheatMode");
		this.playerName = input.getString("playerName");
	}
	
	//写字段值
	public override void write(BmobOutput output, Boolean all)
	{
		base.write(output, all);
		
		output.Put("score", this.score);
		output.Put("cheatMode", this.cheatMode);
		output.Put("playerName", this.playerName);
	}
}