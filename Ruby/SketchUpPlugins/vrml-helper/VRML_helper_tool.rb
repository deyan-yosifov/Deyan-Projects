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
		alert(getVrmlExportContent)		
		return
		
		path = UI.savepanel ("Save VRML", nil, "vrml_extrude_export.txt")
		
		if(!path.nil?) then
			file = File.new(path, "w")
			file.print getVrmlExportContent()
			file.close
			
			alert("File successfully exported!")
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
		header = '	Shape{
		appearance Appearance {
			material Material {
				diffuseColor 1.0 0.5 1.0
			}
		}		
		geometry Extrusion {
'

		crossSection = getCrossSection()
		spineAndOrientations = getSpineAndOrientations()

		footer = '		}
	}'
				
		return header + crossSection + spineAndOrientations + footer
	end
	
	def getCrossSection
		header = '			crossSection [
'

		matrica = getFaceTransformMatrix()
		entity = @selectedFace
		vnum = entity.vertices.length
		vcur = 1
		pointsText = '';
		textOffset = "				"
		firstPointText = ""
		
		entity.vertices.each { |vrah|      
			newvrah = vrah.position
			newvrah.transform! matrica
			pointsText = pointsText + textOffset + getNumberText(newvrah[0]) + " " + getNumberText(newvrah[1])
			
			if(vcur == 1) then 
				firstPointText = firstPointText + pointsText + '
'
			end
			
			pointsText = pointsText + ',
'			
			vcur+=1
		}
		
		footer = '
		]
'

		return header + pointsText + firstPointText + footer
	end
	
	def getNumberText(coordinateNumber)
		number = coordinateNumber.to_m
		
		numAbs = number.abs		
		if(numAbs < 1E-6) then number = 0.0 end
		
		return number.to_s
	end
	
	def getFaceTransformMatrix
		entity = @selectedFace
		nula = Geom::Point3d.new (0,0,0)
		ppp = Geom::Point3d.linear_combination 1.0, nula, 1.0, @selectedVertex.position
		
		v0 = entity.vertices[0].position
		v1 = entity.vertices[1].position
		kk = entity.normal
		jj = v0.vector_to v1
		jj.normalize!
		ii = jj.cross kk
		ppp = Geom::Point3d.linear_combination 1.0, nula, 1.0, v0

	   #######                       	#x	#y	#z 	##
		matrica = Geom::Transformation.new ([ii[0],	ii[1],	ii[2],	0,  	# i'
										jj[0],	jj[1],	jj[2],	0,  	# j'
										kk[0],	kk[1],	kk[2],	0,  	# k'
										ppp[0],	ppp[1],	ppp[2],	1]) 	# translation
		
		matrica.invert!
		
		return matrica
	end
	
	def getSpineAndOrientations
		headerSpine = '			spine [
'
		footer = '			]
'
		headerOrientation = '			orientation [
'
		textOffset = '				'
		spineCoordinatesText = ''
		orientationCoordinatesText = ''
		
		currentVertex = @selectedVertex
		nextVertex = nil
		edges = @selectedEdges
		endEdgeIndex = edges.length - 1
		currentEdgeIndex = 0
		previousOrientation = nil;
		
		edges.each { |edge|
			spineCoordinatesText = spineCoordinatesText + textOffset + getExportedVertexText(currentVertex) + ',
'		
			v0 = edge.vertices[0]
			v1 = edge.vertices[1]
			
			if(currentVertex == v0) then
				nextVertex = v1
			elsif(currentEdgeIndex == v1) then
				nextVertex = v0
			else
				spineCoordinatesText += "CANNOT FIND MATCHING VERTICES IN EDGEINDEX = " + currentEdgeIndex.to_s
				break
			end
			
			currentOrientation = currentVertex.position.vector_to nextVertex.position
			actualOrientation = nil
			
			if(previousOrientation.nil?) then
				actualOrientation = currentOrientation
			else
				actualOrientation = previousOrientation + currentOrientation
			end
			
			orientationCoordinatesText = orientationCoordinatesText + textOffset + getExportedOrientationText(actualOrientation) + ',
'
									
			previousOrientation = currentOrientation
			currentVertex = nextVertex
			currentEdgeIndex += 1
		}		

		spineCoordinatesText = spineCoordinatesText + textOffset + getExportedVertexText(currentVertex) + '
'			
		orientationCoordinatesText = orientationCoordinatesText + textOffset + getExportedOrientationText(previousOrientation) + '
'
		
		return headerSpine + spineCoordinatesText + footer + headerOrientation + orientationCoordinatesText + footer
	end
	
	def getExportedVertexText(vertex)
		pos = vertex.position
		
		return getNumberText(pos[0]) + " " + getNumberText(pos[1]) + " " + getNumberText(pos[2])
	end
	
	def getExportedOrientationText(orientation)
		unity = orientation.normalize
		
		return getNumberText(unity[0]) + " " + getNumberText(unity[1]) + " " + getNumberText(unity[2]) + " 0.0"
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