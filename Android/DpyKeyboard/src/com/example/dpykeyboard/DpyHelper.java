package com.example.dpykeyboard;

public class DpyHelper {

	private static final String Owner = "DEYAN: ";
	private static final String NotOwner = "!" + DpyHelper.Owner;
	
	public DpyHelper() {
		// TODO Auto-generated constructor stub
	}
	
	public static void myLog(String text){
		DpyHelper.log(DpyHelper.Owner, text);
	}
	
	public static void otherLog(String text){
		DpyHelper.log(DpyHelper.NotOwner, text);
	}
	
	private static void log(String owner, String text){
		android.util.Log.v(owner, text);
	}

}
