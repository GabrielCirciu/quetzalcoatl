# QUETZALCOATL
A private repo for managing the project files without the possibility of completely losing them

# To do:

# Network:
	On player join, spawn them in for other clients
	For new clients, spawn all existing players in
	Synchronize position and rotation of each player

# UI:
	Add every menu button, slider, dropdown
	Set up every aspect to be functional
	Add in-game menu, with full functionality

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

# Set up Character creation:
	Set up full character creation UI functionality with Create/Delete
	Implement character customization and saving it to a file/ loading it
	Character customizer should include for now: Character Name, Base Color
	Saving/Loading should be handled by an identifier: Name <UID>