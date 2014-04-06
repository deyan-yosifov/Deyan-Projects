package com.example.dpykeyboard;

import java.util.ArrayList;
import java.util.List;

import com.example.dpykeyboard.BgKeyboard.BgKey;

import android.content.Context;
import android.content.res.Resources;
import android.content.res.XmlResourceParser;
import android.inputmethodservice.Keyboard;
import android.inputmethodservice.Keyboard.Key;
import android.inputmethodservice.Keyboard.Row;

public class BgKeyboard extends Keyboard {

	public static final int SHIFT_KEY_CODE = -1;
	public static final int ENTER_KEY_CODE = 10;
	private boolean isShiftOn = false;
	private List<Key> alphabetKeys;
	
	public BgKeyboard(Context context, int xmlLayoutResId) {
		super(context, xmlLayoutResId);
		// TODO Auto-generated constructor stub
	}

	public BgKeyboard(Context context, int xmlLayoutResId, int modeId) {
		super(context, xmlLayoutResId, modeId);
		// TODO Auto-generated constructor stub
	}

//	public BgKeyboard(Context context, int xmlLayoutResId, int modeId,
//			int width, int height) {
//		super(context, xmlLayoutResId, modeId, width, height);
//		// TODO Auto-generated constructor stub
//	}

	public BgKeyboard(Context context, int layoutTemplateResId,
			CharSequence characters, int columns, int horizontalPadding) {
		super(context, layoutTemplateResId, characters, columns, horizontalPadding);
		// TODO Auto-generated constructor stub
	}
	
	@Override
    protected Key createKeyFromXml(Resources res, Row parent, int x, int y, XmlResourceParser parser) {
        BgKey key = new BgKey(res, parent, x, y, parser);
        this.tryAddAlphabetKey(key);
        return key;
    }
	
	private void tryAddAlphabetKey(Key key){
		CharSequence label = key.label;
		if(label != null && label.length() == 1 && Character.isLetter(label.charAt(0))){
			if(this.alphabetKeys == null){
				this.alphabetKeys = new ArrayList<Key>();
			}
			this.alphabetKeys.add(key);
			DpyHelper.Log("after add to alphabets: " + label);
		}
	}
	
	public boolean getShiftState(){
		return this.isShiftOn;
	}
	
	public void changeShiftState() {
		this.isShiftOn = !this.isShiftOn;
		this.setShifted(this.isShiftOn);
		
		for(int index = 0; index < this.alphabetKeys.size(); index++){
			Key key = this.alphabetKeys.get(index);
			char letter = key.label.charAt(0);
			key.codes[0] = this.isShiftOn ? Character.toUpperCase(letter) : Character.toLowerCase(letter);	
		}
	}
	
	static class BgKey extends Keyboard.Key {
        
        public BgKey(Resources res, Keyboard.Row parent, int x, int y, XmlResourceParser parser) {
            super(res, parent, x, y, parser);
        }
        
        /**
         * Overriding this method so that we can reduce the target area for the key that
         * closes the keyboard. 
         */
        @Override
        public boolean isInside(int x, int y) {
//        	int actualY = codes[0] == KEYCODE_CANCEL ? y - 10 : y;
            return super.isInside(x, y);
        }
        
        public boolean isStartingWithCode(int keyCode){
        	return this.codes != null && 
        			this.codes.length > 0 &&
        			this.codes[0] == keyCode;
        }
    }

}
