/**
 * 
 */
package com.example.dpykeyboard;

import android.inputmethodservice.InputMethodService;
import android.inputmethodservice.KeyboardView;
import android.view.KeyEvent;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodSubtype;

public class BgKeyboardService extends InputMethodService 
		implements KeyboardView.OnKeyboardActionListener {

	private KeyboardView inputView;
	private BgKeyboard bgPhoneticsSmallKeyboard;    
    private BgKeyboard currentKeyboard;
	private int lastDisplayWidth;
	
	/**
	 * Constructor
	 */
	public BgKeyboardService() {
		// TODO Auto-generated constructor stub
	}

	@Override
	public View onCreateCandidatesView() {
		DpyHelper.Log("onCreateCandidatesView");
		// TODO Auto-generated method stub
		return super.onCreateCandidatesView();
	}

	@Override
	public View onCreateInputView() {
		DpyHelper.Log("onCreateInputView");
		
		this.inputView = (KeyboardView) getLayoutInflater().inflate(R.layout.input, null);
        this.inputView.setOnKeyboardActionListener(this);
        this.inputView.setKeyboard(this.bgPhoneticsSmallKeyboard);
        return this.inputView;
	}
	
	/**
     * This is the point where you can do all of your UI initialization.  It
     * is called after creation and any configuration change.
     */
    @Override public void onInitializeInterface() {
    	DpyHelper.Log("onInitializeInterface");
    	
        if (this.currentKeyboard != null) {
            // Configuration changes can happen after the keyboard gets recreated,
            // so we need to be able to re-build the keyboards if the available
            // space has changed.
            int displayWidth = this.getMaxWidth();
            if (displayWidth == this.lastDisplayWidth) return;
            this.lastDisplayWidth = displayWidth;
        }
        
        this.bgPhoneticsSmallKeyboard = new BgKeyboard(this, R.xml.bg_phonetics_small);
    }

	@Override
	public void onFinishInput() {
		DpyHelper.Log("onFinishInput");
		// TODO Auto-generated method stub
		super.onFinishInput();
	}
	
	@Override
	public void onStartInput(EditorInfo attribute, boolean restarting) {
		DpyHelper.Log("onStartInput");
		// TODO Auto-generated method stub
		super.onStartInput(attribute, restarting);
	}

	@Override
	public void onKey(int primaryCode, int[] keyCodes) {
		DpyHelper.Log("onKey; primaryCode: " + primaryCode);
		getCurrentInputConnection().commitText(
                String.valueOf((char) primaryCode), 1);
		
	}

	@Override
	public void onPress(int primaryCode) {
		DpyHelper.Log("onPress; primaryCode: " + primaryCode);		
	}

	@Override
	public void onRelease(int primaryCode) {
		DpyHelper.Log("onRelease; primaryCode: " + primaryCode);	
		
	}

	@Override
	public void onText(CharSequence text) {
		DpyHelper.Log("onText; text: " + text);	
		
	}

	@Override
	public void swipeDown() {
		DpyHelper.Log("swipeDown");	
		
	}

	@Override
	public void swipeLeft() {
		DpyHelper.Log("swipeLeft");	
		
	}

	@Override
	public void swipeRight() {
		DpyHelper.Log("swipeRight");	
		
	}

	@Override
	public void swipeUp() {
		DpyHelper.Log("swipeUp");			
	}

}
