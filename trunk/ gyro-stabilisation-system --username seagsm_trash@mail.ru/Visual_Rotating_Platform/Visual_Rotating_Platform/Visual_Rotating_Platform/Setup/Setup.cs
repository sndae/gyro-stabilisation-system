using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/* Serialisation and files processing. */
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

/* Work with XML. */
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;



namespace FormSetup

{  
    /* SFP Setup Form Parameters. */
    public class SFP
    {
        /* List of FormParameters. */
        public List<FormParameters> p_FormSetupList;
        /* Name of form setup file. */
        public string SetupFileFileName;
        public string ComPortName;


        /* Definition of Form Setup Paramerets. */
        public class FormParameters
        {
            public string FormName          { get; set; }
            public int    FormWidth         { get; set; }
            public int    FormHeight        { get; set; }
            public int    FormPosition_Left { get; set; }
            public int    FormPosition_Top  { get; set; }
            public string FormComPortName   { get; set; }
        }

        /* Write setup parameters data to setup file. */
        public void WriteSetupFile(Form CurrentForm)
        {
            /* Here we will save form data and save it to file. */
            List<FormSetup.SFP.FormParameters> sfpp = new List<FormSetup.SFP.FormParameters>();

            /* Set setup filenasme. */
            FileInfo SetupFile = new FileInfo(SetupFileFileName);
            /* If setup file exist, let read it .*/
            if (SetupFile.Exists == false)
            {
                /* In this case we should create FormParameters structure and add it to the List.*/
                FormSetup.SFP.FormParameters my_current_form = new FormSetup.SFP.FormParameters();
                my_current_form.FormName          = CurrentForm.Text;
                /* It can be negative if form is minimised. */
                if (CurrentForm.Left >= 0)
                {
                    my_current_form.FormPosition_Left = CurrentForm.Left;
                    
                    my_current_form.FormHeight = CurrentForm.Height;
                    my_current_form.FormWidth = CurrentForm.Width;
                }
                if (CurrentForm.Top >= 0)
                {
                    my_current_form.FormPosition_Top = CurrentForm.Top;
                }

                if (p_FormSetupList == null)
                {
                    p_FormSetupList = new List<FormParameters>(); 
                }
                p_FormSetupList.Add(my_current_form);
            }
            else
            {
                /* Put setup file name in FormSetup.SFP structure. */
                SetupFileFileName = SetupFile.Name;
                /* Read setup from the file. */
                FormSetup.SFP.FormParameters results = ReadSetupFile(CurrentForm);

                /* If name already existwe should add form parameters. */
                if (results != null)
                {
                    /* In this case all data writed directli to List structure.*/
                    if(CurrentForm.Left >= 0)
                    {
                        results.FormPosition_Left = CurrentForm.Left;
                        /* If form minimised, do not chanche size of form. */
                        results.FormHeight = CurrentForm.Height;
                        results.FormWidth = CurrentForm.Width;
                    }
                    if (CurrentForm.Top >= 0)
                    {
                        results.FormPosition_Top = CurrentForm.Top;
                    }    
                    results.FormComPortName  = ComPortName;
                }
                else
                {
                    /* In this case we should create FormParameters structure and add it to the List.*/
                    FormSetup.SFP.FormParameters my_current_form = new FormSetup.SFP.FormParameters();
                    my_current_form.FormName = CurrentForm.Text;
                    if (CurrentForm.Left >= 0)
                    {
                        my_current_form.FormPosition_Left = CurrentForm.Left;

                        my_current_form.FormHeight = CurrentForm.Height;
                        my_current_form.FormWidth = CurrentForm.Width;

                    }
                    if (CurrentForm.Top >= 0)
                    {
                        my_current_form.FormPosition_Top = CurrentForm.Top;
                    }                    
                    my_current_form.FormComPortName = ComPortName;
                    if (p_FormSetupList == null)
                    {
                        p_FormSetupList = new List<FormParameters>();
                    }
                    p_FormSetupList.Add(my_current_form);
                }
            }

            /* Create new or rewrite setup file with new parameters. */
            FileStream fs = new FileStream(SetupFileFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            /* Create serialiser */
            XmlSerializer sr = new XmlSerializer(p_FormSetupList.GetType());
            /* Serialisation. */
            sr.Serialize(fs, p_FormSetupList);
            fs.Close();
        }


        /* Read setup parameters data from setup file. */
        public FormSetup.SFP.FormParameters ReadSetupFile(Form CurrentForm)
        {   
            /* Create new Form Setup List element. */
            p_FormSetupList = new List<FormParameters>();
 
            FileInfo SetupFile = new FileInfo(SetupFileFileName);
            /* If setup file exist, let read it .*/
            if (SetupFile.Exists == false)
            {
                return(null);
            }
            /* Create XML elements from a source file. */
            XElement xTree = XElement.Load(@SetupFileFileName);
            /* Create an enumerable collection of the elements. */
            IEnumerable<XElement> elements = xTree.Elements();
            /* Evaluate each element and set set values in the book object. */
            foreach (XElement el in elements)
            {
                /* Create local example of Form Setup Parameters element. */
                FormSetup.SFP.FormParameters fp = new FormSetup.SFP.FormParameters();
                /* Read and parse fields from XML file. */
                IEnumerable<XElement> props = el.Elements();
                /* Search Form parameters. */
                foreach (XElement p in props)
                {
                    if (p.Name.ToString() == "FormName")
                    {
                        try
                        {
                            fp.FormName = p.Value;
                        }
                        catch (ArgumentException exp)
                        {
                            MessageBox.Show(exp.Message);
                            fp.FormName = null;
                        }
                    }
                    else if (p.Name.ToString() == "FormWidth")
                    {
                        try
                        {
                            fp.FormWidth = int.Parse(p.Value);
                        }
                        catch (ArgumentException exp)
                        {
                            MessageBox.Show(exp.Message);
                            fp.FormWidth = 300;
                        }
                    }
                    else if (p.Name.ToString() == "FormHeight")
                    {
                        try
                        {
                            fp.FormHeight = int.Parse(p.Value);
                        }
                        catch (ArgumentException exp)
                        {
                            MessageBox.Show(exp.Message);
                            fp.FormHeight = 300;
                        }
                    }
                    else if (p.Name.ToString() == "FormPosition_Left")
                    {
                        try
                        {
                            fp.FormPosition_Left = int.Parse(p.Value);
                        }
                        catch (ArgumentException exp)
                        {
                            MessageBox.Show(exp.Message);
                            fp.FormPosition_Left = 100;
                        }
                    }
                    else if (p.Name.ToString() == "FormPosition_Top")
                    {
                        try
                        {
                            fp.FormPosition_Top = int.Parse(p.Value);
                        }
                        catch (ArgumentException exp)
                        {
                            MessageBox.Show(exp.Message);
                            fp.FormPosition_Top = 100;
                        }
                    }
                    else if (p.Name.ToString() == "FormComPortName")
                    {
                        try
                        {
                            fp.FormComPortName = p.Value;
                        }
                        catch (ArgumentException exp)
                        {
                            MessageBox.Show(exp.Message);
                            fp.FormComPortName = "COM1";
                        }
                    }
                    //
                    //Here we can add more parameters.
                    //
                }//foreach (XElement p in props)
                /* Add Form Setup Element to list. */
                p_FormSetupList.Add(fp);
            }//foreach (XElement el in elements)

            /* Find current form name in setup file. */
            FormSetup.SFP.FormParameters results = p_FormSetupList.Find
            (
                delegate(FormSetup.SFP.FormParameters bk)
                {
                    return bk.FormName == CurrentForm.Text;
                }
            );
            
            return results;    
        }//public void ReadSetupFile()

    }// end of "public class SFP"
}
