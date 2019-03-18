import bpy
import json
import os
from pathlib import Path

def createCurvedText(text, radius, textBoxWidth):

	# Circle creation
	bpy.ops.curve.primitive_bezier_circle_add(radius=radius, location=(0, 0, 0))
	circle=bpy.context.object
	#circle.hide = True


	# Text creation
    # Place the text in the middle of the axis using the textBoxWidth
    # inverted as the text's left borders are at x = 0
	bpy.ops.object.text_add(location=(-textBoxWidth / 2 ,0,0))

	textObject=bpy.context.object
	textObject.data.size = 0.05 # "Font-size"
	textObject.data.align_x = 'JUSTIFY'
	textObject.data.align_y = 'CENTER'
	textObject.data.text_boxes[0].width = textBoxWidth
	textObject.data.body = text

	font=bpy.data.fonts.load(filepath="C:\\Windows\\Fonts\\arial.ttf")
	textObject.data.font = font

	# Check to see if the material exists
	materialName = "Black_font_colour"
	materialNameWasFound = False

	# Only create the black material once
	for mat in bpy.data.materials:
	    if mat.name == materialName:
	        material = mat
	        materialNameWasFound = True
	        break

	if not materialNameWasFound:
	    mat = bpy.data.materials.new(name=materialName)


	textObject.data.materials.append(mat)
	bpy.context.object.active_material.diffuse_color = (0, 0, 0) # Black


	bpy.ops.object.modifier_add(type='CURVE')
	bpy.context.object.modifiers["Curve"].object = circle
	bpy.ops.transform.rotate(value=1.5708, axis=(1, 0, 0)) # Rotate by 90 degrees along the X-axis


	# set circle as parent object to make it easier for spawning multiple instanced of these elements
	#textObject.parent = circle
	return


# Gets the text to create a mesh out of from the JSON formatted file.
def getTextFromFile():	
# File path of the blender file
    filepath = bpy.data.filepath
# File path of the text data file (JSON formatted)
    jsonFilePath = Path(os.path.dirname(filepath)).parents[0] / "StreamingAssets/text_data.json"
    #print(jsonFilePath)
    
    jsonFile = open(str(jsonFilePath), 'r')
    data = json.load(jsonFile)
    return data


## Main


bpy.ops.object.select_all(action='TOGGLE')
bpy.ops.object.select_all(action='TOGGLE')
bpy.ops.object.delete()


data = getTextFromFile()

for Text in data['Items']:
    if Text['Id'] == 1:
        i = 0;
        for textBoxWidth in range (2, 8):
            createCurvedText(Text['Text'], 1, textBoxWidth * 0.25)
#for modifier in range(1, 5):
#	 (radius, textBoxWidth)
#	createCurvedText(data['Items']['Text'][0], modifier * 0.5, modifier * 0.75)
