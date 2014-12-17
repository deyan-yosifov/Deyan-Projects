class DpyVrmlHelperTool

	def initialize 
		cursorPath = Sketchup.find_support_file ("DpyCursor.png", "Plugins/DPY_VRML_HELPER_TOOL/")
		@@cursor = UI.create_cursor(cursorPath, 5, 5)	
		
		@states = ["Select face to extrude", "Pick extrude start point", "Select extrude path", "Save extrude VRML code"]
		@stateIndex = 0
		@canMoveToNextState = false
		@selectedFace = nil
		@selectedVertex = nil
		@selectedEdges = Array.new
	end
   
	def onSetCursor	
		UI.set_cursor(@@cursor)
	end
	
	def activate
		showMessage
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
		if(@stateIndex == 0) then #"Select face to extrude"
			ph = view.pick_helper
			ph.do_pick x,y
			entity = ph.best_picked
			
			if(entity.typename == "Face") then
				alert("picked a face")
			end
		end
		if(@stateIndex == 1) then #"Pick extrude start point"
			inputPoint = view.inputpoint x,y		
			vertex = inputPoint.vertex
			if(!vertex.nil?) then
				alert("picked vertex")
			end
		end
		if(@stateIndex == 2) then #"Select extrude path"
			ph = view.pick_helper
			ph.do_pick x,y
			entity = ph.best_picked
			
			if(entity.typename == "Edge") then
				alert("picked a edge")
			end
		end
		if(@stateIndex == 3) then #"Save extrude VRML code"
			path = UI.savepanel ("Saveni ne6to", nil, "vrml_extrude_export.txt")
			if(!path.nil?) then
				file = File.new(path, "w")
				file.print pr
				file.close
			end
		end
	end

	def onKeyDown(key, repeat, flags, view)   
		#ENTER key == 13
		if (key == 13) then
			if(@stateIndex == 0) then #"Select face to extrude"
				ph = view.pick_helper
				ph.do_pick x,y
				entity = ph.best_picked
				
				if(entity.typename == "Face") then
					alert("picked a face")
				end
			end
			if(@stateIndex == 1) then #"Pick extrude start point"
				
			end
			if(@stateIndex == 2) then #"Select extrude path"
				
			end
			if(@stateIndex == 3) then #"Save extrude VRML code"
				
			end
		end	 
	  
	end
   
	def moveToNextState
		if(canMoveToNextState) then
			@stateIndex++
			if @stateIndex >= @states.length then @stateIndex = 0 end
			showMessage
		else
			alert("Cannot move to next state before making the correct selection!")
		end
	end
	
	def showMessage		
		@msg = @states[@stateIndex].to_s
		Sketchup::set_status_text(@msg) 
	end
	
	def alert(text)
		UI.messagebox (text)
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