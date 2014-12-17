class DpyVrmlHelperTool

	def initialize 
		cursorPath = Sketchup.find_support_file ("DpyCursor.png", "Plugins/DPY_VRML_HELPER_TOOL/")
		@@cursor = UI.create_cursor(cursorPath, 5, 5)	
		
		@states = ["Select face to extrude and press ENTER to continue", "Pick extrude start point and press ENTER to continue", "Select extrude path and press ENTER to continue", "Save extrude VRML code or press ESCAPE to start from the beginning"]
		initStates()
	end
	
	def initStates
		@stateIndex = 0
		@selectedFace = nil
		@selectedVertex = nil
		@selectedEdges = Array.new	
	end
   
	def onSetCursor	
		UI.set_cursor(@@cursor)
	end
	
	def activate
		initStates()
		showMessage()
	end
	
	def draw(view)
		#Nothing here
	end
	
	def deactivate(view)
		#Nothing here
	end
	
	def onCancel(reason, view)
		if(@stateIndex == 0) then #"Select face to extrude"
			@selectedFace = nil
		end
		if(@stateIndex == 1) then #"Pick extrude start point"
			@selectedVertex = nil
		end
		if(@stateIndex == 2) then #"Select extrude path"
			@selectedEdges = Array.new
		end
		if(@stateIndex == 3) then #"Save extrude VRML code"
			activate()
		end	
		
		clearSelection()
	end
	
	def onUserText(text, view)
		#Nothing here
	end
	
	def onLButtonDown(flags, x, y, view)
		if(@stateIndex == 0) then #"Select face to extrude"
			ph = view.pick_helper
			ph.do_pick x,y
			entity = ph.best_picked
			
			if(!entity.nil? && entity.typename == "Face") then
				@selectedFace = entity
				selectNew(entity)
			else			
				alert("Face not picked!")
			end
		end
		if(@stateIndex == 1) then #"Pick extrude start point"
			inputPoint = view.inputpoint x,y		
			vertex = inputPoint.vertex
			if(!vertex.nil?) then
				@selectedVertex = vertex
				alert("picked vertex: " + vertex.position.to_s)
			else			
				alert("Vertex not picked!")
			end
		end
		if(@stateIndex == 2) then #"Select extrude path"
			ph = view.pick_helper
			ph.do_pick x,y
			entity = ph.best_picked
			
			if(!entity.nil? && entity.typename == "Edge") then
				@selectedEdges.push(entity)
				select(entity)
			else			
				alert("Edge not picked!")
			end
		end
		if(@stateIndex == 3) then #"Save extrude VRML code"
			alert("press escape to start new export")
		end
	end

	def onKeyDown(key, repeat, flags, view)   
		#ENTER key == 13
		if (key == 13) then
			if(@stateIndex < 3) then #"Select face, vertex and edges"
			 tryMoveToNextState()
			end
			if(@stateIndex == 3) then #"Save extrude VRML code"
				exportVrml()
			end
		end	 	  
	end
	
	def exportVrml
		path = UI.savepanel ("Save VRML", nil, "vrml_extrude_export.txt")
		
		if(!path.nil?) then
			file = File.new(path, "w")
			file.print getVrmlExportContent()
			file.close
		end
	end
   
	def tryMoveToNextState
		if(canMoveToNextState()) then
			@stateIndex = @stateIndex + 1
			statesCount = @states.length
			if @stateIndex >= statesCount then
				@stateIndex = 0 
			end
			
			showMessage()
			clearSelection()
		else
			alert("Cannot move to next state before making the correct selection!")
		end
	end
	
	def canMoveToNextState		
		canMoveVar = true
	
		if(@stateIndex == 0) then #"Select face to extrude"
			canMoveVar = !@selectedFace.nil?
		end
		if(@stateIndex == 1) then  #"Pick extrude start point"
			canMoveVar = !@selectedVertex.nil?
		end
		if(@stateIndex == 2) then #"Select extrude path"
			canMoveVar = @selectedEdges.length > 0
		end
		
		canMoveVar
	end
	
	def getVrmlExportContent
		return "sample vrml"
	end
	
	def showMessage		
		@msg = @states[@stateIndex].to_s
		Sketchup::set_status_text(@msg) 
	end
	
	def alert(text)
		UI.messagebox (text)
	end
	
	def select(entity)
		Sketchup.active_model.selection.add(entity)
	end
	
	def selectNew(entity)
		clearSelection()
		select(entity)
	end
	
	def clearSelection
		Sketchup.active_model.selection.clear()
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