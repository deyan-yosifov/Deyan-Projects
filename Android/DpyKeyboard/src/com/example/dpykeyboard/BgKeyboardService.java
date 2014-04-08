/**
 * 
 */
package com.example.dpykeyboard;

import android.content.res.Configuration;
import android.inputmethodservice.InputMethodService;
import android.inputmethodservice.KeyboardView;
import android.view.KeyEvent;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodSubtype;

public class BgKeyboardService extends InputMethodService 
		implements KeyboardView.OnKeyboardActionListener {

	private static final boolean DEBUG = true;
	private KeyboardView inputView;
	private BgKeyboard bgPhoneticsKeyboard;    
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
    public void onConfigurationChanged(Configuration newConfig) {
    DpyHelper.Log("BgKeyboard: onConfigurationChanged");

    if(BgKeyboardService.DEBUG) {
	    /* now let's wait until the debugger attaches */
	    android.os.Debug.waitForDebugger();
    }
   
    super.onConfigurationChanged(newConfig);
   
    /* do something useful... */
           
    }

	@Override
	public View onCreateInputView() {
		DpyHelper.Log("onCreateInputView");
		
		this.inputView = (KeyboardView) getLayoutInflater().inflate(R.layout.input, null);
        this.inputView.setOnKeyboardActionListener(this);
        this.inputView.setKeyboard(this.currentKeyboard);
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
            DpyHelper.Log("onInitializeInterface displayWidth" + displayWidth);
            if (displayWidth == this.lastDisplayWidth) return;
            this.lastDisplayWidth = displayWidth;
        }
        
        this.bgPhoneticsKeyboard = new BgKeyboard(this, R.xml.bg_phonetics);
        this.currentKeyboard = this.bgPhoneticsKeyboard;
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
		this.setCurrentKeyboard(this.bgPhoneticsKeyboard);
		// TODO Auto-generated method stub
		super.onStartInput(attribute, restarting);
	}
	
	private void setCurrentKeyboard(BgKeyboard keyboard){
		if(this.currentKeyboard == keyboard){
			return;
		}
		
		this.currentKeyboard = keyboard;
		this.inputView.setKeyboard(this.currentKeyboard);
	}

	@Override
	public void onKey(int primaryCode, int[] keyCodes) {
		DpyHelper.Log("onKey; primaryCode: " + primaryCode);
		String textToCommit = "";
		
		if(primaryCode == BgKeyboard.SHIFT_KEY_CODE){
			this.currentKeyboard.changeShiftState();
			this.inputView.invalidateAllKeys();
		} else if(primaryCode > 0){
			textToCommit = String.valueOf((char)primaryCode);
		}
		
		if(textToCommit.length() > 0){
			this.getCurrentInputConnection().commitText(textToCommit, textToCommit.length());
		}
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
