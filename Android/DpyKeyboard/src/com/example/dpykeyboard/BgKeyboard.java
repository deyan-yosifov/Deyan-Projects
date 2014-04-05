package com.example.dpykeyboard;

import com.example.dpykeyboard.BgKeyboard.BgKey;

import android.content.Context;
import android.content.res.Resources;
import android.content.res.XmlResourceParser;
import android.inputmethodservice.Keyboard;
import android.inputmethodservice.Keyboard.Key;
import android.inputmethodservice.Keyboard.Row;

public class BgKeyboard extends Keyboard {

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
		super(context, layoutTemplateResId, characters, columns,
				horizontalPadding);
		// TODO Auto-generated constructor stub
	}
	
	@Override
    protected Key createKeyFromXml(Resources res, Row parent, int x, int y, 
            XmlResourceParser parser) {
        Key key = new BgKey(res, parent, x, y, parser);
        if (key.codes[0] == 10) {
            // set something to this Enter key
        }
        return key;
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
    }

}
