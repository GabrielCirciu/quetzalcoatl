# QUETZALCOATL
A public repo for managing the project files with other memebers of the team
A multiplayer survival game based on steams' new Socket Servers and based on a Mayan culture in the skies.

# To do:

# Network:
	Synchronize lerping between position and rotation to match a tickrate

# UI:
	Add remaining menu buttons, sliders, dropdowns
	Give more functionaility to said options

# File Management:
	Update and improve File Manager script to handles all data file saves
	Set up temporary world and character creation just for foldering purposes
	Main Folder will be: ../AppData/LocalLow/%Company/%Game/.. (persistentPath)
	Options Save file will be: ../%Game/Config.cfg
	Other Save files/folders will be: ../%Game/Saves/..
	Character's Folders will be: ../Saves/Characters/..
	World's Folders will be: ../Saves/Worlds/..
	Character <ANY> Data file will be: ../Characters/Character_Name_UID/%Data.dat
	World's Data Folders will be: ../Worlds/%World_Name_UID/..
	World <ANY> Data file will be: ../%World_Name_UID/%Data.dat

# Character Creation:
	Set up full character creation UI functionality with Create/Delete
	Implement character customization and saving it to a file/ loading it
	Character customizer should include for now: Character Name, Base Color
	Saving/Loading should be handled by an identifier: Name <UID>
