/**
 * 
 */
package com.example.dpykeyboard;

import android.content.res.Configuration;
import android.inputmethodservice.InputMethodService;
import android.inputmethodservice.Keyboard;
import android.inputmethodservice.KeyboardView;
import android.view.KeyEvent;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.ExtractedText;
import android.view.inputmethod.ExtractedTextRequest;
import android.view.inputmethod.InputConnection;
import android.view.inputmethod.InputMethodSubtype;

public class BgKeyboardService extends InputMethodService 
		implements KeyboardView.OnKeyboardActionListener {

	private static final boolean DEBUG = true;
	private KeyboardView inputView;
	private BgKeyboard bgPhoneticsKeyboard;    
    private BgKeyboard currentKeyboard;
	private int lastDisplayWidth;
	private int lastSelectionStart;
	private int lastSelectionEnd;
	
	/**
	 * Constructor
	 */
	public BgKeyboardService() {
		// TODO Auto-generated constructor stub
	}

	@Override
	public View onCreateCandidatesView() {
		DpyHelper.myLog("onCreateCandidatesView");
		// TODO Auto-generated method stub
		return super.onCreateCandidatesView();
	}
	
	@Override
    public void onConfigurationChanged(Configuration newConfig) {
    DpyHelper.myLog("BgKeyboard: onConfigurationChanged");

    if(BgKeyboardService.DEBUG) {
	    /* now let's wait until the debugger attaches */
	    android.os.Debug.waitForDebugger();
    }
   
    super.onConfigurationChanged(newConfig);
   
    /* do something useful... */
           
    }	

	@Override
	public View onCreateInputView() {
		DpyHelper.myLog("onCreateInputView");
		
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
    	DpyHelper.myLog("onInitializeInterface");
    	
        if (this.currentKeyboard != null) {
            // Configuration changes can happen after the keyboard gets recreated,
            // so we need to be able to re-build the keyboards if the available
            // space has changed.
            int displayWidth = this.getMaxWidth();
            DpyHelper.myLog("onInitializeInterface displayWidth" + displayWidth);
            if (displayWidth == this.lastDisplayWidth) return;
            this.lastDisplayWidth = displayWidth;
        }
        
        this.bgPhoneticsKeyboard = new BgKeyboard(this, R.xml.bg_phonetics);
        this.currentKeyboard = this.bgPhoneticsKeyboard;
    }

	@Override
	public void onFinishInput() {
		DpyHelper.myLog("onFinishInput");
		// TODO Auto-generated method stub
		super.onFinishInput();
        if (this.inputView != null) {
        	this.inputView.closing();
        }
	}
	
	@Override
	public void onStartInput(EditorInfo attribute, boolean restarting) {
		DpyHelper.myLog("onStartInput");
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
		DpyHelper.myLog("onKey; primaryCode: " + primaryCode);
		String textToCommit = "";
		
		if(primaryCode == BgKeyboard.SHIFT_KEY_CODE){
			this.handleShift();
		}else if(primaryCode == BgKeyboard.ENTER_KEY_CODE){
			this.handleEnter();
		}else if (primaryCode == Keyboard.KEYCODE_DELETE) {
            handleBackspace();
        } else if (primaryCode == Keyboard.KEYCODE_CANCEL) {
            this.handleClose();
            return;
        } else if(primaryCode > 0){
			textToCommit = String.valueOf((char)primaryCode);
		}
		
		if(textToCommit.length() > 0){
			this.getCurrentInputConnection().commitText(textToCommit, textToCommit.length());
		}
	}	

	@Override
	public void onUpdateExtractedText(int token, ExtractedText text) {	
		DpyHelper.myLog("onUpdateExtractedText(" + token + ", " + text + ")");
		// TODO Auto-generated method stub
		super.onUpdateExtractedText(token, text);
	}
	
	
	
	/**
     * Deal with the editor reporting movement of its cursor.
     */
    @Override public void onUpdateSelection(int oldSelStart, int oldSelEnd,
            int newSelStart, int newSelEnd,
            int candidatesStart, int candidatesEnd) {
        super.onUpdateSelection(oldSelStart, oldSelEnd, newSelStart, newSelEnd,
                candidatesStart, candidatesEnd);

        DpyHelper.myLog("onUpdateSelection " + oldSelStart + " " + oldSelEnd + " "
        		+ newSelStart + " " + newSelEnd + " "
        		+ candidatesStart + " " + candidatesEnd + " ");
        
        this.lastSelectionStart = newSelStart;
        this.lastSelectionEnd = newSelEnd;
//		InputConnection connection = this.getCurrentInputConnection();		
//        connection.setSelection(newSelStart, newSelEnd);
    }
	
	private void handleEnter(){
		this.keyDownUp(KeyEvent.KEYCODE_ENTER);
	}
	
	private void handleBackspace(){
		InputConnection connection = this.getCurrentInputConnection();	
		
		connection.beginBatchEdit();		
		
		int lastSelectionLength= this.lastSelectionEnd - this.lastSelectionStart;
		int before = lastSelectionLength == 0 ? 1 : 0;
		int after = lastSelectionLength;		
		connection.setSelection(this.lastSelectionStart, this.lastSelectionStart);
		connection.deleteSurroundingText(before, after);
		
		connection.endBatchEdit();	

		DpyHelper.myLog("deleteSurroundingText(" + before + ", " + after + ")");
	}
	
	
	
	
	private void handleClose(){
        requestHideSelf(0);
        this.inputView.closing();
	}
	
	private void handleShift(){
		this.currentKeyboard.changeShiftState();
		this.inputView.invalidateAllKeys();
	}
	
	private void keyDownUp(int keyEventCode) {
        getCurrentInputConnection().sendKeyEvent(
                new KeyEvent(KeyEvent.ACTION_DOWN, keyEventCode));
        getCurrentInputConnection().sendKeyEvent(
                new KeyEvent(KeyEvent.ACTION_UP, keyEventCode));
    }

	@Override
	public void onPress(int primaryCode) {
		DpyHelper.myLog("onPress; primaryCode: " + primaryCode);		
	}

	@Override
	public void onRelease(int primaryCode) {
		DpyHelper.myLog("onRelease; primaryCode: " + primaryCode);	
		
	}

	@Override
	public void onText(CharSequence text) {
		DpyHelper.myLog("onText; text: " + text);	
		
	}

	@Override
	public void swipeDown() {
		DpyHelper.myLog("swipeDown");	
		
	}

	@Override
	public void swipeLeft() {
		DpyHelper.myLog("swipeLeft");	
		
	}

	@Override
	public void swipeRight() {
		DpyHelper.myLog("swipeRight");	
		
	}

	@Override
	public void swipeUp() {
		DpyHelper.myLog("swipeUp");			
	}

}
