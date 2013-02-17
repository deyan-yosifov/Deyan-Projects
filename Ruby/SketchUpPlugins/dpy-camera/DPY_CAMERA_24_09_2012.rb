require 'Win32API'

class Animation
   def initialize
     @@DPY = DPYmoveCAMERAtool.new
     @camera = Sketchup.active_model.active_view.camera
	 @time = Sketchup.active_model.active_view.average_refresh_time * 10
	 @v = @@DPY.velocity
	 @msg = ""
	 @newEye = Geom::Point3d.new
	 @newTarget = Geom::Point3d.new
   end
 
   def nextFrame(view)
   
     @camera = view.camera
	 @time = view.average_refresh_time * 10
	 @v = @@DPY.velocity
	 
	 if (@@DPY.vk_up)
	     @newEye = Geom::Point3d.linear_combination 1.0, @camera.eye, @v*@time, @camera.zaxis.to_a
         @newTarget = Geom::Point3d.linear_combination 1.0, @camera.target, @v*@time, @camera.zaxis.to_a	 
         view.camera.set @newEye, @newTarget, [0,0,1]
	 end
	 
	 if (@@DPY.vk_down)
	     @newEye = Geom::Point3d.linear_combination 1.0, @camera.eye, -@v*@time, @camera.zaxis.to_a
         @newTarget = Geom::Point3d.linear_combination 1.0, @camera.target, -@v*@time, @camera.zaxis.to_a	 
         view.camera.set @newEye, @newTarget, [0,0,1] 
	 end
	 
	 if (@@DPY.vk_left)
	     @newEye = Geom::Point3d.linear_combination 1.0, @camera.eye, -@v*@time, @camera.xaxis.to_a
         @newTarget = Geom::Point3d.linear_combination 1.0, @camera.target, -@v*@time, @camera.xaxis.to_a
         view.camera.set @newEye, @newTarget, [0,0,1]  
	 end
	 
	 if (@@DPY.vk_right)
         @newEye = Geom::Point3d.linear_combination 1.0, @camera.eye, @v*@time, @camera.xaxis.to_a
         @newTarget = Geom::Point3d.linear_combination 1.0, @camera.target, @v*@time, @camera.xaxis.to_a	
         view.camera.set @newEye, @newTarget, [0,0,1] 
	 end
	 @msg = "Velocity = " + (@v*0.0254).to_s + " m/s. Position in space --> " + view.camera.eye[0].to_s + " " + view.camera.eye[1].to_s + " " + view.camera.eye[2].to_s
	 if(!@@DPY.mouseCorrected) then @msg = @msg + ". Click for MouseCorrection!" end
	 if(@@DPY.mouseCorrected)
	     @msg = @msg + ". Esc for GunMode On/Off: " 
		 if(@@DPY.eraser) then @msg = @msg + "On" end
		 if(!@@DPY.eraser) then @msg = @msg + "Off" end
	 end	 
	 if(!@@DPY.animate) 
	     @msg = ""
	 end
	 Sketchup::set_status_text(@msg)
	 view.show_frame
	 return @@DPY.animate
   end

end

class DPYmoveCAMERAtool

   def initialize 
	 cursorPath2 = Sketchup.find_support_file ("DpyCameraCursor2.png", "Plugins/DPY_CAMERA/")
	 @@cursor2 = UI.create_cursor(cursorPath2, 5, 5)	
	 #cursorPath1 = Sketchup.find_support_file ("DpyCameraCursor1.png", "Plugins/DPY_CAMERA/")
	 #@@cursor1 = UI.create_cursor(cursorPath1, 5, 5)
   end
   
    def onSetCursor	
     UI.set_cursor(@@cursor2)
    end


   def activate
     camera = Sketchup.active_model.active_view.camera
	 newTarget = Geom::Point3d.linear_combination 1.0, camera.eye, 1.0, camera.zaxis.to_a
     camera.set camera.eye, newTarget, [0,0,1]

     @@mouseDouble = false  
	 @@boom = false
	 @@eraser = false
	 @@mouseCorrected = false
     @@vk_up = false
	 @@vk_down = false
	 @@vk_left = false
	 @@vk_right = false
     @firstPress = true
	 @@animate = true
	 @c = Sketchup.active_model.active_view.center
     @corX = 0
     @corY = 0	 	 
	 @@v = 6/0.0254
	 @msg = "Use arrow keys and mouse to navigate! Velocity = " + (@@v*0.0254).to_s + " m/s. Change it by typing a desired velocity and pressing Enter."
	 Sketchup::set_status_text(@msg)
	 	 
	 setCursorPos = Win32API.new("user32", "SetCursorPos", ['I', 'I'], 'V')
	 setCursorPos.Call(@c[0]+@corX,@c[1]+@corY)	 
   end
   
   def draw(view)
     if(@@eraser and @@mouseCorrected)
		 view.drawing_color = "red"
		 d = 5
		 l = 25
		 view.draw2d GL_LINES, [@c[0],@c[1] - d,0],[@c[0],@c[1] - l,0] , [@c[0],@c[1] + d,0],[@c[0],@c[1] + l,0] , [@c[0] - d,@c[1],0],[@c[0] - l,@c[1],0] , [@c[0] + d,@c[1],0],[@c[0] + l,@c[1],0]
		 if(@@boom) then status = view.draw_text [@c[0] + d,@c[1] + d,0], "BOOM!" end
		 if(@@mouseDouble = true)
			 @@boom = false
			 @@mouseDouble = false
		 end
	 end
   end
   
   def deactivate(view)
     @@animate = false
   end
   
   def onCancel(reason, view)
     @@eraser = !@@eraser
   end
   
   
   def onUserText(text, view)
     if(text.to_f) then @@v = text.to_f/0.0254 end
   end
   
   def onLButtonDoubleClick(flags, x, y, view)
       if(@@eraser and @@mouseCorrected)
	     @@mouseDouble = true
	     @@boom = true
		 ph = view.pick_helper
		 ph.do_pick x,y
		 best = ph.best_picked
		 if(!best.deleted?) then best.erase! end
	   end	   
   end
  
   def onLButtonDown(flags, x, y, view)
       if(!@@mouseCorrected)
	     @@mouseCorrected = true
		 @corX = @c[0] - x
		 @corY = @c[1] - y
	   end   
       if(@@eraser and @@mouseCorrected)
	     @@boom = true
		 ph = view.pick_helper
		 ph.do_pick x,y
		 best = ph.best_picked
		 if(!best.deleted?) then best.erase! end
	   end
	   
=begin	  ### Dictionaries and Attributes ###
	   if(!@@eraser and @@mouseCorrected)
		 ph = view.pick_helper
		 ph.do_pick x,y
		 best = ph.best_picked
		 inputpoint = view.inputpoint x, y
		 if(!best.deleted?)
		    best.set_attribute best.typename, "test", inputpoint.position
			best.attribute_dictionaries.each { |dict|
			    print=""
				dict.each { |key,value|
					print += dict.name.to_s + " --> " + key.to_s + " = " + value.to_s + "\n"
				}
				UI.messagebox(print)
			}			
		 end
	   end
=end	   

   end #end of LButtonDown
   
   def onLButtonUp(flags, x, y, view)
     @@boom = false
   end
   
   
   def onMouseMove(flags, x, y, view)
   
     getCursorPos = Win32API.new("user32", "GetCursorPos", ['P'], 'V')
	 lpPoint = " " * 8 # store two LONGs
	 getCursorPos.Call(lpPoint)	 
	 p, q = lpPoint.unpack("LL") # get the actual values
     ray = view.pickray p-@corX, q-@corY
	  
	 status = ray[1].samedirection? [0,0,1]
	 if(!status)       
	   view.camera.set ray[0], ray[1], [0,0,1]
	 end
	 
	 setCursorPos = Win32API.new("user32", "SetCursorPos", ['I', 'I'], 'V')
	 setCursorPos.Call(@c[0]+@corX,@c[1]+@corY)
     
   end
   
   
   def onKeyDown(key, repeat, flags, view)   	 
   
     if(@firstPress)
	     @firstPress = false
		 Sketchup.active_model.active_view.animation =  Animation.new
	 end
 
	 if (key == VK_UP) then @@vk_up = true end	     
	 if (key == VK_DOWN) then @@vk_down = true end 
	 if (key == VK_RIGHT) then @@vk_right = true end
	 if (key == VK_LEFT) then @@vk_left = true end	 
	  
   end
   
   
   def onKeyUp(key, repeat, flags, view)
     if (key == VK_UP) then @@vk_up = false end 
	 if (key == VK_DOWN) then @@vk_down = false end 
	 if (key == VK_LEFT) then @@vk_left = false end 
	 if (key == VK_RIGHT) then @@vk_right = false end 
   end
   
   def animate
     @@animate
   end
   
   def vk_up
     @@vk_up
   end
   
   def vk_down
     @@vk_down
   end
   
   def vk_left
     @@vk_left
   end

   def vk_right
     @@vk_right
   end
   
   def velocity
     @@v
   end
   
   def mouseCorrected
     @@mouseCorrected
   end
   
   def eraser
     @@eraser
   end  

end # end class DPYmoveCAMERAtool definition


######Creating new toolbar item
toolbar = UI::Toolbar.new "DPY_CAMERA"


######Adding new command to the toolbar item
DPY_CAMERA = UI::Command.new("DPY_CAMERA") { 

     Sketchup.active_model.start_operation 'DPY_CAMERA', true     
	 Sketchup.active_model.select_tool DPYmoveCAMERAtool.new
     Sketchup.active_model.commit_operation

 }
DPY_CAMERA.small_icon = File.join("DPY_CAMERA", "DPY_CAMERA_small.jpg")
DPY_CAMERA.large_icon = File.join("DPY_CAMERA", "DPY_CAMERA_large.jpg")

toolbar = toolbar.add_item DPY_CAMERA
toolbar.show