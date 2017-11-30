selection = Sketchup.active_model.selection
#model = Sketchup.active_model.entities

###################### Na4alo definicii Dipiuay

### sumirane na vektori
def sum_point(to4ki)
 suma = Geom::Point3d.new (0,0,0)
 to4ki.each { |to4ka| 
  suma = Geom::Point3d.linear_combination 1.0, suma, 1.0, to4ka.to_a
 }
 return suma
end



### umnojavane na vektor s 4islo
def mul_point(to4k,num)
 sum = Geom::Point3d.new (0,0,0)
 sum = Geom::Point3d.linear_combination 1.0, sum, num, to4k.to_a 
 return sum
end



###tetrahedron definiciq
def tetra (i)
 vector=i.normal
 rabove=i.edges
 varhove = i.vertices

 a=0
 rabove.each { |rab|
  a=a+rab.length
 }
 a=a/3
 vector.length = a* Math.sqrt(2.0/3.0)
 br=0
 tt=Array.new
 varhove.each { |vrah|
 tt[br]=vrah.position
 br+=1
 }
 centar = Geom::Point3d.new
 centar = sum_point(tt)
 centar = mul_point(centar,1.0/3.0)
 vrah = Geom::Point3d.linear_combination 1.0, centar, 1.0, vector.to_a
 lica = Array.new
 br=0
 while(br<3)
  lica[br] = Sketchup.active_model.entities.add_face vrah,varhove[br%3].position,varhove[(br+1)%3].position
  test=vector.dot lica[br].normal
  if (test< 0)
   lica[br].reverse!
  end
  br+=1
 end
 i.reverse!

end


### Octahedron definiciq
def octa (i)
 vector=i.normal
 rabove=i.edges
 varhove = i.vertices

 a=0
 rabove.each { |rab|
  a=a+rab.length
 }
 a=a/3
 vector.length = a* Math.sqrt(1.0/6.0)
 br=0
 tt=Array.new
 varhove.each { |vrah|
 tt[br]=vrah.position
 br+=1
 }
 centar = Geom::Point3d.new
 centar = sum_point(tt)
 centar = mul_point(centar,1.0/3.0)
 ba6_centar = Geom::Point3d.linear_combination 1.0, centar, 1.0, vector.to_a
 tt[5] = Geom::Point3d.linear_combination 2.0, ba6_centar, -1.0, tt[0]
 br=3
 while (br<5)
  tt[br] = Geom::Point3d.linear_combination 2.0, ba6_centar, -1.0, tt[(br-2)]
  br+=1
 end

 lica = Array.new
 br=0
 while (br<4)
  temp = Geom::Point3d.linear_combination -1.0, ba6_centar, 1.0, tt[(br%4)+1]

  if (br>0)
   lica[br-1] = Sketchup.active_model.entities.add_face tt[0],tt[(br%4)+1],tt[(br+1)%4+1]
   test=temp.to_a.dot lica[br-1].normal
   if (test< 0)
     lica[br-1].reverse!
   end 
  end

   lica[br+3] = Sketchup.active_model.entities.add_face tt[5],tt[(br%4)+1],tt[(br+1)%4+1]
   test=temp.to_a.dot lica[br+3].normal
   if (test< 0)
     lica[br+3].reverse!
   end 

  br+=1
 end
 i.reverse!

end




### Icosahedron definiciq
def icosa (i)
 vector=i.normal
 rabove=i.edges
 varhove = i.vertices

 a=0
 rabove.each { |rab|
  a=a+rab.length
 }
 a=a/3
 fi=Math.sqrt(5.0)
 fi=(fi+1)/2
 vector.length = a* Math.sqrt((fi*fi*3-1)/12)
 br=0
 tt=Array.new
 varhove.each { |vrah|
 tt[br]=vrah.position
 br+=1
 }
 centar = Geom::Point3d.new
 centar = sum_point(tt)
 centar = mul_point(centar,1.0/3.0)
 ba6_centar = Geom::Point3d.linear_combination 1.0, centar, 1.0, vector.to_a
 tt[11] = Geom::Point3d.linear_combination 2.0, ba6_centar, -1.0, tt[0]
 ii = Geom::Point3d.linear_combination 0.5, tt[1], 0.5, tt[2]
 ii = Geom::Point3d.linear_combination 1.0, ii, -1.0, ba6_centar
 jj = Geom::Point3d.linear_combination 1.0, tt[2], -1.0, tt[1]
 kk = Geom::Point3d.linear_combination -1.0/fi, ii, 1.0, tt[0]
 kk = Geom::Point3d.linear_combination 1.0, kk, -1.0, ba6_centar

 tt[4] = Geom::Point3d.linear_combination -2.0/fi, ii, 1.0, tt[0]
 tt[3] = Geom::Point3d.linear_combination 1.0/fi, kk, 1.0, ba6_centar
 tt[3] = Geom::Point3d.linear_combination fi/2.0, jj, 1.0, tt[3]
 tt[5] = Geom::Point3d.linear_combination -1.0*fi, jj, 1.0, tt[3]

 br=6
 while (br<11)
  tt[br] = Geom::Point3d.linear_combination 2.0, ba6_centar, -1.0, tt[(br-5)]
  br+=1
 end

 lica = Array.new
 br=0
 while (br<5)
  temp1 = Geom::Point3d.linear_combination -1.0, ba6_centar, 1.0, tt[(br%5)+1]
  temp2 = Geom::Point3d.linear_combination -1.0, ba6_centar, 1.0, tt[(br%5)+6]

  if (br>0)
   lica[br-1] = Sketchup.active_model.entities.add_face tt[0],tt[(br%5)+1],tt[(br+1)%5+1]
   test=temp1.to_a.dot lica[br-1].normal
   if (test< 0)
     lica[br-1].reverse!
   end 
  end

   lica[br+4] = Sketchup.active_model.entities.add_face tt[(br+3)%5+6],tt[(br%5)+1],tt[(br+1)%5+1]
   test=temp1.to_a.dot lica[br+4].normal
   if (test< 0)
     lica[br+4].reverse!
   end

   lica[br+9] = Sketchup.active_model.entities.add_face tt[11],tt[(br%5)+6],tt[(br+1)%5+6]
   test=temp2.to_a.dot lica[br+9].normal
   if (test< 0)
     lica[br+9].reverse!
   end 

   lica[br+14] = Sketchup.active_model.entities.add_face tt[(br+3)%5+1],tt[(br%5)+6],tt[(br+1)%5+6]
   test=temp2.to_a.dot lica[br+14].normal
   if (test< 0)
     lica[br+14].reverse!
   end 

  br+=1
 end
 i.reverse!

end
 

### Delene definiciq
def delen (i,deleniq)
 varhove = i.vertices
 normal1 = i.normal
 br=0
 tt=Array.new
varhove.each { |vrah|
 tt[br]=vrah.position
 br+=1
 }
 i.erase!
 deleniq=deleniq.to_f
 dd=Array.new
dd[0]=tt[0]
dd[deleniq]=tt[1]
dd[2*deleniq]=tt[2]

 br=0
 while (br < deleniq-1)
  temp1 = (deleniq-1)-br
  temp2 = br+1
  dd[br+1] = Geom::Point3d.linear_combination temp1/deleniq, dd[0], temp2/deleniq, dd[deleniq]

  dd[2*deleniq - (1+br)] = Geom::Point3d.linear_combination temp1/deleniq, dd[2*deleniq], temp2/deleniq, dd[deleniq]

  br+=1
 end

  lice = Sketchup.active_model.entities.add_face dd[deleniq - 1], dd[deleniq + 1], dd[deleniq]
  normal2 = lice.normal
  status = normal1.samedirection? normal2

  if (!status)
   lice=lice.reverse!
  end


 br=0
 while (br < deleniq-1)
   brr = 0
   granica = deleniq-br
   granica2 = granica - 1
   while (brr < granica)
     temp1 = granica - brr
     temp2 = granica2 - brr
     temp3 = brr + 1
     pp1 = Geom::Point3d.linear_combination temp1/granica, dd[br], brr/granica, dd[2*deleniq - br]
     pp2 = Geom::Point3d.linear_combination temp2/granica, dd[br], temp3/granica, dd[2*deleniq-br]
     pp3 = Geom::Point3d.linear_combination temp2/granica2, dd[br+1], brr/granica2, dd[2*deleniq - br-1]
     lice = Sketchup.active_model.entities.add_face pp1, pp2, pp3
     normal2 = lice.normal
     status = normal1.samedirection? normal2

     if (!status)
        lice=lice.reverse!
      end

      if(brr < granica-1)
        temp1 = temp2 - 1
        temp2 = brr + 1
        pp4 = Geom::Point3d.linear_combination temp1/granica2, dd[br+1], temp2/granica2, dd[2*deleniq - br-1]
        lice = Sketchup.active_model.entities.add_face pp4, pp2, pp3
        status = normal1.samedirection? normal2

        if (!status)
         lice=lice.reverse!
        end
      end

     brr+=1
   end

  br+=1
 end

end


### definiciq na okolna obvivka
def out_points (i)
 normal1 = i.normal

 loop = i.outer_loop   
 edgeuses = loop.edgeuses
 broi = edgeuses.length
 rabove = Array.new

  br=0
  while (br < broi)
   rabove[br] = edgeuses[br].edge
   br+=1
  end



 r=0
 br=0
 out_p = Array.new

  while (br < broi) 
    cur = br%broi
    nex = (br+1)%broi
    p1 = rabove[cur].start.position
    p2 = rabove[cur].end.position
    pn1 = rabove[nex].start.position
    pn2 = rabove[nex].end.position

    if (p1 == pn1)
     pp = p1
     pp1 = p2
     pp2 = pn2
    end
   
    if (p1 == pn2)
     pp = p1
     pp1 = p2
     pp2 = pn1
    end

    if (p2 == pn1)
     pp = p2
     pp1 = p1
     pp2 = pn2
    end

    if (p2 == pn2)
     pp = p2
     pp1 = p1
     pp2 = pn1
    end

    vector1 = pp1.vector_to pp
    vector2 = pp.vector_to pp2
    status = vector1.samedirection? vector2
    
    if (!status)
     out_p[r]=pp
     r+=1
    end
 
 #   UI.messagebox (cur.to_s + p1.to_s + p2.to_s + "\n" + nex.to_s + pn1.to_s + pn2.to_s + "\n" + pp1.to_s + "\n" + pp.to_s + "\n" + pp2.to_s + "\n" + (!status).to_s , MB_MULTILINE)
    br+=1 
  end 

# UI.messagebox (r.to_s + "\n" + out_p.to_s)
 fff = Sketchup.active_model.entities.add_face out_p
 normal2 = fff.normal
 status = normal1.samedirection? normal2

 if (!status)
   fff=fff.reverse!
 end

 return fff


end


####################### Krai definicii Dipiuay


	

######Dobawqne na toolbar
toolbar = UI::Toolbar.new "Ravnostranni"


######Dobawqne na komanda tetrahedron kam toolbara
tetrahedron = UI::Command.new("tetrahedron") { 

Sketchup.active_model.start_operation 'tetrahedron', true


countf=0
erase_face=6
erase_face=UI.messagebox ("Do you want to erase bottom faces?", MB_YESNO) #optional

 selection.each { |entity| 
  if entity.typename == "Face"
    entity = out_points (entity)
  end

  if entity.typename == "Face" and entity.edges.length == 3 

    countf+=1
    
    tetra (entity)

    if erase_face==6
      entity.erase!
    end
  end
   }

UI.messagebox (countf.to_s + " tetrahedra added.") #optional

Sketchup.active_model.commit_operation

 }
tetrahedron.small_icon = File.join("RAVNOSTRANNI_DIR", "Tetrahedron_small.png")
tetrahedron.large_icon = File.join("RAVNOSTRANNI_DIR", "Tetrahedron_large.png")

######Dobawqne na komanda octahedron kam toolbara
octahedron = UI::Command.new("octahedron") { 

Sketchup.active_model.start_operation 'octahedron', true


countf=0
erase_face=6
erase_face=UI.messagebox ("Do you want to erase bottom faces?", MB_YESNO) #optional

 selection.each { |entity| 
  if entity.typename == "Face"
    entity = out_points (entity)
  end

  if entity.typename == "Face" and entity.edges.length == 3 

    countf+=1
    
    octa (entity)

    if erase_face==6
      entity.erase!
    end
  end
   }

UI.messagebox (countf.to_s + " octahedra added.") #optional

Sketchup.active_model.commit_operation

 }
octahedron.small_icon = File.join("RAVNOSTRANNI_DIR", "Octahedron_small.png")
octahedron.large_icon = File.join("RAVNOSTRANNI_DIR", "Octahedron_large.png")

######Dobawqne na komanda icosahedron kam toolbara
icosahedron = UI::Command.new("icosahedron") { 

Sketchup.active_model.start_operation 'icosahedron', true


countf=0
erase_face=6
erase_face=UI.messagebox ("Do you want to erase bottom faces?", MB_YESNO) #optional

 selection.each { |entity| 
  if entity.typename == "Face"
    entity = out_points (entity)
  end

  if entity.typename == "Face" and entity.edges.length == 3 

    countf+=1
    
    icosa (entity)

    if erase_face==6
      entity.erase!
    end
  end
   }

UI.messagebox (countf.to_s + " icosahedra added.") #optional

Sketchup.active_model.commit_operation


 }
icosahedron.small_icon = File.join("RAVNOSTRANNI_DIR", "Icosahedron_small.png")
icosahedron.large_icon = File.join("RAVNOSTRANNI_DIR", "Icosahedron_large.png")



######Dobawqne na komanda delene kam toolbara
delene = UI::Command.new("delene") { 

Sketchup.active_model.start_operation 'delene', true




 prompts = ["devisions"] 
 defaults = [4]  
 deleniq = UI.inputbox prompts, defaults, "Enter number of devisions."

 selection.each { |entity| 
  if entity.typename == "Face"
    entity = out_points (entity)
  end

  if entity.typename == "Face" and entity.edges.length == 3     
    delen (entity, deleniq[0].to_f)
  end
   }


Sketchup.active_model.commit_operation


 }
delene.small_icon = File.join("RAVNOSTRANNI_DIR", "Delene_small.png")
delene.large_icon = File.join("RAVNOSTRANNI_DIR", "Delene_large.png")


toolbar = toolbar.add_item tetrahedron
toolbar = toolbar.add_item octahedron
toolbar = toolbar.add_item icosahedron
toolbar = toolbar.add_item delene
toolbar.show




############################################


#Dobawqne na menu gore pri menutata
# Add a menu item to launch our plugin.
#UI.menu("Plugins").add_item("Proba s UI") {
#}



#Dobavqne na desen buton
# UI.add_context_menu_handler do |context_menu|  
#  context_menu.add_separator  
#  context_menu.add_item("Obvivka") { 
#    Sketchup.active_model.start_operation 'delene', true 
 
#   selection.each { |entity| 
#    if entity.typename == "Face"     
#     ff=out_points(entity) 
#     ll= ff.edges.length
#     UI.messagebox(ll)
#    end
#   }

#Sketchup.active_model.commit_operation
#  } 
# end
