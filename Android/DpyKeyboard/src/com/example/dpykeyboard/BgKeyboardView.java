package com.example.dpykeyboard;

import android.content.Context;
import android.inputmethodservice.Keyboard;
import android.inputmethodservice.KeyboardView;
import android.inputmethodservice.Keyboard.Key;
import android.util.AttributeSet;

public class BgKeyboardView extends KeyboardView {

	static final int KEYCODE_OPTIONS = -100;
	
	public BgKeyboardView(Context context, AttributeSet attrs) {
		super(context, attrs);
		// TODO Auto-generated constructor stub
	}

	public BgKeyboardView(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
		// TODO Auto-generated constructor stub
	}

	@Override
    protected boolean onLongPress(Key key) {
//        if (key.codes[0] == Keyboard.KEYCODE_CANCEL) {
//            getOnKeyboardActionListener().onKey(KEYCODE_OPTIONS, null);
//            return true;
//        } else {
            return super.onLongPress(key);
//        }
    }
}
