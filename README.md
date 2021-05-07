# QUETZALCOATL
A private repo for managing the project files without the possibility of completely losing them

# To do:

# Network:
	Look into data serialization to send over the network (json or other format)
	(might be easier to handle multiple kinds of code data)

# Set up data filtering and synchronization:
	Data labeled at first (or first two) bytes of the data array
	Data label will differentiate between chat, position, rotation
	Server only handles distribution of data
	Client handles decoding data and acting according to data label

# Set up Server Creation UI and savings:
	When hosting server, Server Name should be given
	Server should handle saving all data in a folder / files

# Set up, and update previous, file management/ saving:
	Update and improve File Manager script to handles all data file saves
	(Set up temporary world creation just for foldering purposes)
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

# Set up Character sync over server:
	Implement server sync of custom character features to be loaded on join
	Server should handle keeping track of all players (position/rotation/customization)
	All tracked player data is sent to new client on join
	Client should save all server related data periodically to keep track at re-joining
	Data includes: ServerWorldID, Position, Rotation, Character Features
