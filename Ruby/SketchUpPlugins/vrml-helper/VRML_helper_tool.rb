class DpyVrmlHelperTool

	def initialize 
		cursorPath = Sketchup.find_support_file ("DpyCursor.png", "Plugins/DPY_VRML_HELPER_TOOL/")
		@@cursor = UI.create_cursor(cursorPath, 5, 5)	
	end
   
	def onSetCursor	
		UI.set_cursor(@@cursor)
	end
	
	def activate
		@msg = "Select extrude face!"
		Sketchup::set_status_text(@msg) 
	end
	
	def draw(view)
		#Nothing here
	end
	
	def deactivate(view)
		#Nothing here
	end
	
	def onCancel(reason, view)
		#Nothing here
	end
	
	def onUserText(text, view)
		#Nothing here
	end
	
   def onLButtonDown(flags, x, y, view)
		#ph = view.pick_helper
		#ph.do_pick x,y
		#best = ph.best_picked		
		
		inputPoint = view.inputpoint x,y
		
		vertex = inputPoint.vertex
		
		if(!vertex.nil?) then		
			UI.messagebox ("picked vertex: " + vertex.to_s)
		else
			UI.messagebox ("picked NOT vertex: " + inputPoint.to_s)
		end
   end
end #end class DpyVrmlHelperTool

######Adding tool in toolbar
toolbar = UI::Toolbar.new "DPY_VRML_HELPER_TOOLBAR"

######Adding command in the toolbar
DPY_VRML_HELPER_COMMAND = UI::Command.new("DPY_VRML_HELPER_COMMAND") { 

     Sketchup.active_model.start_operation 'DPY_VRML_HELPER_COMMAND', true     
	 Sketchup.active_model.select_tool DpyVrmlHelperTool.new
     Sketchup.active_model.commit_operation

 }
DPY_VRML_HELPER_COMMAND.small_icon = File.join("DPY_VRML_HELPER_TOOL", "DPY_VRML_HELPER_BUTTON_small.jpg")
DPY_VRML_HELPER_COMMAND.large_icon = File.join("DPY_VRML_HELPER_TOOL", "DPY_VRML_HELPER_BUTTON_large.jpg")

toolbar = toolbar.add_item DPY_VRML_HELPER_COMMAND
toolbar.show