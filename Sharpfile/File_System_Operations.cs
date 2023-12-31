﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
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


        public Task<bool> Move_Or_Rename_File(string path); // DONE
        public Task<bool> Move_Or_Rename_Directory(string path); // DONE
        public Task<bool> Copy_File(string path); // TO BE DONE;
        public Task<bool> Copy_Directory(string path); // TO BE DONE;
        public Task<bool> List_Files(); // DONE
        public Task<bool> Navigate_To_Directory(string directory_path); // DONE
        public Task<bool> Navigate_To_Previos_Directory(); // DONE
        public Task<bool> Create_Directory(string directory_path); // DONE
        public Task<bool> Delete_Directory(string directory_path); // DONE
        public Task<bool> Search_File(string file_name); //DONE
        public Task<bool> Open_Current_Directory_In_Terminal(); //DONE
        public Task<bool> Delete_File(string file_path); // DONE
        public Task<bool> Open_File(string file_path);// DONE
    }
}
