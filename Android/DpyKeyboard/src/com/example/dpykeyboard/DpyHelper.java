package com.example.dpykeyboard;

public class DpyHelper {

	private static final String Owner = "DEYAN: ";
	
	public DpyHelper() {
		// TODO Auto-generated constructor stub
	}
	
	public static void Log(String text){
		android.util.Log.v(DpyHelper.Owner, text);
	}

}
