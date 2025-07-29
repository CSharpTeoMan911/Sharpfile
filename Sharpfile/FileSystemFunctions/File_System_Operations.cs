namespace Sharpfile.FileSystemFunctions
{
    internal interface File_System_Operations
    {
        ////////////////////////////////////////////
        //                 TO DO                  //
        ////////////////////////////////////////////
        //                                        //
        //  - MOVE FILE OR DIRECTORY OPERATION    //
        //                                        //
        //  - RENAME FILE OR DIRECTORY OPERATION  //
        //                                        //
        //  - COPY FILE OR DIRECTORY              //
        //                                        //
        ////////////////////////////////////////////


        public bool Move_Or_Rename_File(string path); // DONE
        public bool Move_Or_Rename_Directory(string path); // DONE
        public bool Copy_File(string path); // TO BE DONE;
        public bool Copy_Directory(string path); // TO BE DONE;
        public bool List_Files(); // DONE
        public bool Navigate_To_Directory(string directory_path); // DONE
        public bool Navigate_To_Previos_Directory(); // DONE
        public bool Create_Directory(string directory_path); // DONE
        public bool Delete_Directory(string directory_path); // DONE
        public bool Search_File(string file_name); //DONE
        public bool Open_Current_Directory_In_Terminal(); //DONE
        public bool Delete_File(string file_path); // DONE
        public bool Open_File(string file_path);// DONE
    }
}
