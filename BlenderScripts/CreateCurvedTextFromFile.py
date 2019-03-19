import bpy
import json
import os
from pathlib import Path

def createCircle(radius):
    # Circle creation
    bpy.ops.curve.primitive_bezier_circle_add(radius=radius, location=(0, 0, 0))
    circle=bpy.context.object
    circle.name = "Radius-" + str(radius)
    #circle.hide = True
    return circle

def createCurvedText(text, textBoxWidth, circleObj):  
	# Text creation
    # Place the text in the middle of the axis using the textBoxWidth
    # inverted as the text's left borders are at x = 0
    bpy.ops.object.text_add(location=(-textBoxWidth/2, 0, 0))
    textObject=bpy.context.object
    textObject.data.size = 0.05 # "Font-size"
    textObject.data.align_x = 'JUSTIFY'
    textObject.data.align_y = 'CENTER'
    textObject.data.text_boxes[0].width = textBoxWidth
    textObject.data.body = text

    # Name to reflect which width was used. Casting to int to avoid
    # floating point rounding errors and casting to string be used as a name.
    textObject.name = "Text" + str(int(textBoxWidth*10))

    # Set the font to Arial
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

    # Add modifier to curve text along the circle
    bpy.ops.object.modifier_add(type='CURVE')
    bpy.context.object.modifiers["Curve"].object = circleObj
    bpy.ops.transform.rotate(value=1.5708, axis=(1, 0, 0)) # Rotate by 90 degrees along the X-axis
    
    # Set circle as parent of the textObject, but remember the data
    textObjWorldMatrix = textObject.matrix_world.copy()
    textObject.parent = circleObj
    textObject.matrix_world = textObjWorldMatrix
    
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


def createCurvedTextsAtDistance(initialWidth, lastWidth, textData, circleObj):
    widthModifier = 0.10 # To do ranges in smaller increments than whole numbers

    for textBoxWidth in range (initialWidth, lastWidth):
        createCurvedText(textData['Text'], textBoxWidth * widthModifier, circleObj)
    
    return


## Main

# Hack to delete everything before creation. Nice for development.
bpy.ops.object.select_all(action='TOGGLE')
bpy.ops.object.select_all(action='TOGGLE')
bpy.ops.object.delete()
###

data = getTextFromFile()


for Text in data['Items']:
    if Text['Id'] == 1:
#        createCurvedTextsAtDistance(2, 7, Text, createCircle(0.5))
        createCurvedTextsAtDistance(2, 20, Text, createCircle(1))
#        createCurvedTextsAtDistance(2, 7, Text, createCircle(1.5))
#        createCurvedTextsAtDistance(2, 7, Text, createCircle(2))
        
        
        #for textBoxWidth in range (2, 8):
        #    createCurvedText(Text['Text'], 0.5, textBoxWidth * 0.25)
            
#for modifier in range(1, 5):
#	 (radius, textBoxWidth)
#	createCurvedText(data['Items']['Text'][0], modifier * 0.5, modifier * 0.75)
