using UnityEngine;
using System.Collections;

public class Gird : MonoBehaviour {

	public int _num;
	public int _row;
	public int _col;

	public int Num
	{
		get { return _num;}
		set { _num = value;}
	}

	public int ROW
	{
		get { return _row;}
		set { _row = value;}
	}

	public int COL
	{
		get { return _col;}
		set { _col = value;}
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	public void setProperty(int num , int row , int col)
	{
		_num = num;
		_row = row;
		_col = col;
	}

	public void swap_rowAndcol(ref Gird other)
	{
		var gird1_row = _row;
		var gird1_col = _col;
		_row = other.ROW;
		_col = other.COL;
		other._row = gird1_row;
		other._col = gird1_col;
	}
}
