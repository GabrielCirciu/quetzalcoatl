# QUETZALCOATL
A public repo for managing the project files with other memebers of the team
Networking tool based on Steam Socket Servers, using the Unity rendering engine

# Features:
	Ability to log into a virtualized world
	Modifiable graphical settings within the main menu
	Possiblity of either joining or hosting a server relayed through the steam socket network
	Connection possible only through Steam server based account system
	Once logged in, while within a server, the ability to interact with other users on the same server
	Feature-rich chat system
	Synchronized position and animation data through all the clients

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
