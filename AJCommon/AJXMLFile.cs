using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;               //XML을 읽고 쓰기 위해서 사용

//환경 설정 파일을 XML 형태로 저장, 읽기

namespace AJParkLib
{
    namespace AJCommon
    {
        public static class AJXMLFile
        {
            private static XmlTextWriter textWriter;
            private static string XML_File_Path = "";

            /// <summary>
            /// XML File 경로 설정
            /// </summary>
            /// <param name="Filepath">XML File Path</param>
            public static void AJ_XML_SET_FILE_PATH(string Filepath)     //XML File경로 설정
            {
                XML_File_Path = Filepath;
            }

            /// <summary>
            /// AJ_XML_SET_FILE_PATH로 설정된 XML File 경로 반환
            /// </summary>
            /// <returns></returns>
            public static string AJ_XML_GET_FILE_PATH()     //XML File경로 설정
            {
                return XML_File_Path;
            }

            /*
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * 작성일 : 2016.12.22 / 1차
             * 작성자 : 김진남
             * XML_CREATE / XML_TEXT_WRITER : XML 파일 생성(By TextWriter, XMLDocument) 2가지
             * XML_ADD    : XML 노드 추가 / XML 속성 추가 
             * XML_REMOVE : XML 노드 삭제 / XML 속성 삭제
             * XML_MODIFY : XML 속성값 변경
             * XML_READ   : XML 속성값 읽기
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * 작성일 : 2016.12.27 / 2차
             * 작성자 : 김진남
             * 내  용 : throw로 에러 처리한것들 주석처리
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             */

            //라이브러리 내부에서 사용하는 함수
            #region Private_Function
            /// <summary>
            /// FilePath를 입력했는지 여부 확인.
            /// return : 정상 -> 1, 비정상 -> 0
            /// </summary>
            /// <param name="temp"></param>
            /// <returns></returns>
            private static int check_File_Path(string temp)
            {
                if (temp.Length < 1)
                {
                    return 0;
                }

                else
                {
                    return 1;
                }
            }

            /// <summary>
            /// Root의 바로 하위 노드 중 Element의 중복을 체크
            /// 중복되는 것이 있으면 0, 없다면 1 리턴
            /// </summary>
            /// <param name="xml_path">XML파일 경로</param>
            /// <param name="element">Element명</param>
            /// <returns></returns>
            private static int check_node(string xml_path, string element)
            {
                XmlDocument Xml_Doc = new XmlDocument();
                Xml_Doc.Load(xml_path);

                //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                if (root.SelectNodes(element).Count > 0)
                {
                    return 0;
                }

                return 1;
            }

            /// <summary>
            /// Root 바로 하위 노드의 자식들 중 Element의 중복을 체크
            /// 중복되는 것이 있으면 0, 없다면 1 리턴
            /// </summary>
            /// <param name="xml_path">XML 파일 경로</param>
            /// <param name="element">Element명</param>
            /// <param name="node_path">Node 경로</param>
            /// <returns></returns>
            private static int check_node(string xml_path, string element, string node_path)
            {

                if (node_path.Equals(""))                               //노드 패스가 공백인 경우
                {
                    return check_node(xml_path, element);               //위의 함수 호출해서 리턴
                }

                else
                {
                    XmlDocument Xml_Doc = new XmlDocument();
                    Xml_Doc.Load(xml_path);

                    //XmlNode root = Xml_Doc.SelectNodes("root")[0];            //수정 전
                    XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트
                    XmlNode now_node = root.SelectSingleNode(node_path);

                    if (now_node == null)
                    {
                        return 1;
                    }

                    else if (now_node.SelectNodes(element).Count > 0)
                    {
                        return 0;
                    }

                    return 1;
                }

                
            }
            #endregion

            #region XML_TEXT_WRITER

            //TextWriter로 XML을 생성
            //코드에서 쉽게 볼 수 있을 것 같지만, 사용을 지양함..

            /// <summary>
            /// textWriter로 XML을 처음 생성
            /// </summary>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_Start()
            {
                try
                {
                    // 생성할 XML 파일 경로와 이름, 인코딩 방식을 설정합니다.
                    textWriter = new XmlTextWriter(XML_File_Path, Encoding.UTF8);

                    // 들여쓰기 설정
                    textWriter.Formatting = Formatting.Indented;

                    // 문서에 쓰기를 시작합니다.
                    textWriter.WriteStartDocument();
                    return 1;
                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// textWriter로 XML을 처음 생성
            /// </summary>
            /// <param name="xml_path">xml파일 경로</param>
            /// /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_Start(string xml_path)
            {
                try
                {
                    // 생성할 XML 파일 경로와 이름, 인코딩 방식을 설정합니다.
                    textWriter = new XmlTextWriter(xml_path, Encoding.UTF8);

                    // 들여쓰기 설정
                    textWriter.Formatting = Formatting.Indented;

                    // 문서에 쓰기를 시작합니다.
                    textWriter.WriteStartDocument();
                    return 1;
                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// Element 입력 선언
            /// </summary>
            /// <param name="str_Element">Element명</param>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_Start_Element(string str_Element)
            {
                try
                {
                    textWriter.WriteStartElement(str_Element);
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// Element 입력 끝
            /// </summary>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_End_Element()
            {
                try
                {
                    textWriter.WriteEndElement();
                    return 1;
                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// Element 입력 끝
            /// </summary>
            /// <param name="str_Element">/Element명</param>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_End_Element(string str_Element)
            {
                try
                {
                    textWriter.WriteEndElement();
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }

            //스트링 쓰기 : <Element>str_Write</Element>
            /// <summary>
            /// <para>
            /// String 입력
            /// </para>
            /// <para>
            /// 입력된 스트링은 [Element]str_Write[/Element]로 들어감
            /// </para>
            /// </summary>
            /// <param name="str_Write">내용</param>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_String(string str_Write)
            {
                try
                {
                    textWriter.WriteString(str_Write);
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }

            //속성 쓰기  : <Element name = "str_value">
            /// <summary>
            /// <para>
            /// Attribute 입력
            /// </para>
            /// <para>
            /// 입력된 속성은 [Element Attribute_name = "Attribute_Value"]로 들어감
            /// </para>
            /// </summary>
            /// <param name="str_Name">Attribute Name</param>
            /// <param name="str_Value">Attribute Value</param>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_Attribute(string str_Name, string str_Value)
            {
                try
                {
                    textWriter.WriteAttributeString(str_Name, str_Value);
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            //XML 쓰기 끝
            /// <summary>
            /// XML 입력 끝 선언
            /// </summary>
            /// <returns>int형 리턴 : 1 -> 성공 0 -> 실패</returns>
            public static int AJ_XML_Writer_End()
            {
                try
                {
                    textWriter.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            #endregion

            #region XML_CREATE
            /// <summary>
            /// <para>
            /// 해당 경로에 "root"노드만을 갖고 있는 XML문서를 새로 만듦
            /// </para>
            /// <example>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// Example-------------------------------------------------
            /// </para>
            /// <code>
            /// <para>
            /// AJ_XML_CREATE("C:\\XML_TEST.XML");
            /// </para>
            /// </code>
            /// <para>
            /// ----------------------------------------------------------
            /// </para>
            /// </example>
            /// </summary>
            /// <param name="xml_path">(String)새로만들 XML파일 경로</param>
            /// <returns>int로 리턴, 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Create(string xml_path)       //root만 갖고 있는 xml을 해당 경로에 새로 만듦
            {
                try
                {
                    if (check_File_Path(xml_path) == 0)                 //파일 경로가 입력되었는지 확인. 나는 입력만 되면 OK. 잘못된 경로면 xml쪽에서 에러 호출해줌
                    {
                        //throw new Exception("XML FILE의 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument newXMLDoc = new XmlDocument();                                      //새로운 XML문서 만들고
                        newXMLDoc.AppendChild(newXMLDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));   //Version = 1.0, 인코딩은 utf-8(한글때문에), 마지막은 standalone으로 https://msdn.microsoft.com/ko-kr/library/ms256048(v=vs.120).aspx 참고.

                        XmlNode root = newXMLDoc.CreateElement("", "root", "");                         //★"root"라는 이름으로 루트 노드 생성. 루트노드 이름이 "root"가 아니면 라이브러리 동작 X, 다른 방법도 찾아봐야지

                        newXMLDoc.Save(xml_path);                                                       //경로에 저장
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }


            /// <summary>
            /// <para>
            /// AJ_XML_Set_Path에서 설정한 경로에 "root"노드만을 갖고 있는 XML문서를 새로 만듦
            /// </para>
            /// <example>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// Example-------------------------------------------------
            /// </para>
            /// <code>
            /// <para>
            /// AJ_XML_CREATE();
            /// </para>
            /// </code>
            /// <para>
            /// ----------------------------------------------------------
            /// </para>
            /// </example>
            /// </summary>
            /// <returns>int로 리턴, 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Create()       //root만 갖고 있는 xml을 해당 경로에 새로 만듦
            {
                try
                {
                    if (check_File_Path(XML_File_Path) == 0)
                    {
                        //throw new Exception("XML FILE의 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument newXMLDoc = new XmlDocument();
                        newXMLDoc.AppendChild(newXMLDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

                        XmlNode root = newXMLDoc.CreateElement("", "root", "");
                        newXMLDoc.AppendChild(root);

                        newXMLDoc.Save(XML_File_Path);
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
                

            }
            #endregion

            #region XML_ADD

            //ADD 하나만 쓰도록 변경....
            //일단 확실치 않아서 지우지 않고 주석처리 해놓음..
            /*
            /// <summary>
            /// <para>
            /// root노드와 연결되는 자식 노드 생성
            /// </para>
            /// </summary>
            /// <param name="xml_path">XML 파일 경로</param>
            /// <param name="Element">생성될 노드의 Element</param>
            /// <param name="attribute_name">생성될 노드의 속성명</param>
            /// <param name="attribute_value">생성될 노드의 속성값</param>
            public static void AJ_XML_Add_Node(string xml_path, string Element, string attribute_name, string attribute_value)
            {
                if (check_File_Path(xml_path) == 0)
                {
                    throw new Exception("XML FILE의 경로를 확인하세요.");
                }

                else
                {
                    if (check_node(xml_path, Element) == 0)
                    {
                        throw new Exception("중복된 Element가 존재합니다.");
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(xml_path);

                        XmlNode root;
                        //root = Xml_Doc.SelectSingleNode("root");      //수정 전
                        root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode newNode;
                        newNode = Xml_Doc.CreateElement(Element);

                        XmlAttribute newAttribute = Xml_Doc.CreateAttribute(attribute_name);
                        newAttribute.Value = attribute_value;

                        newNode.Attributes.Append(newAttribute);

                        root.AppendChild(newNode);

                        Xml_Doc.AppendChild(root);
                        Xml_Doc.Save(xml_path);
                    }
                }
            }

            /// <summary>
            /// root노드와 연결되는 자식 노드 생성
            /// </summary>
            /// <param name="Element">생성될 노드의 Element</param>
            /// <param name="attribute_name">생성될 노드의 속성명</param>
            /// <param name="attribute_value">생성될 노드의 속성값</param>
            public static void AJ_XML_Add_Node(string Element, string name, string value)
            {
                if (check_File_Path(XML_File_Path) == 0)
                {
                    throw new Exception("XML FILE의 경로를 확인하세요.");
                }

                else
                {
                    if (check_node(XML_File_Path, Element) == 0)
                    {
                        throw new Exception("중복된 Element가 존재합니다.");
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(XML_File_Path);

                        XmlNode root;
                        //root = Xml_Doc.SelectSingleNode("root");              //수정 전
                        root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode newNode;
                        newNode = Xml_Doc.CreateElement(Element);

                        XmlAttribute newAttribute = Xml_Doc.CreateAttribute(name);
                        newAttribute.Value = value;

                        newNode.Attributes.Append(newAttribute);

                        root.AppendChild(newNode);

                        Xml_Doc.AppendChild(root);
                        Xml_Doc.Save(XML_File_Path);
                    }
                }
            }
             * */

            //이쪽만 살리는 걸로
            //노드 경로에 "" 이렇게 입력되면 root에 붙일 수 있게 수정해보자

            //root 하단의 노드들에 접근.. 마지막인자에 대한 설명 써야한다
            //public static void AJ_XML_Add_Node(string xml_path, string Element, string name, string value, string node_path)

            /// <summary>
            /// <para>
            /// 자식 노드 생성(꼭 root 노드와 연결되진 않음)
            /// </para>
            /// <example>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// EXAMPLE--------------------------------------------------------------------------------------------------------------
            /// </para>
            /// <para>
            /// root에 Child_1, Child_2라는 자식노드가 있는 상태에서
            /// </para>
            /// <para>
            /// Child_1의 자식 노드를 생성하고 싶다면
            /// </para>
            /// <para>
            /// EX_1 -> AJ_XML_Add_Node("c:\\test.xml", "Child_1", "Grand_Child_1", "att_name", "att_value"); 이다
            /// </para>
            /// <para>
            /// 새로 생성한 Grand_Child_1노드 밑에 또 자식을 생성하고 싶다면
            /// </para>
            /// <para>
            /// EX_2 -> AJ_XML_Add_Node("c:\\test.xml", "Child_1/Grand_Child_1", "Super_Grand_Child_1", "att_name", "att_value");
            /// </para>
            /// <para>
            /// 으로 노드의 부모 경로를 추가하면, 계속 하위로 접근 가능
            /// </para>
            /// <para>
            /// -----------------------------------------------------------------------------------------------------------------------
            /// </para>
            /// </example>
            /// </summary>
            /// <param name="xml_path">XML File 경로</param>
            /// <param name="node_path">생성될 노드의 부모노드 경로</param>
            /// <param name="Element">생성될 노드의 Element</param>
            /// <param name="name">생성될 노드의 속성명</param>
            /// <param name="value">생성될 노드의 속성값</param>
            /// <returns>int형 리턴 -> 1 : 성공, 0 : 실패</returns>
            public static int AJ_XML_Add_Node(string xml_path, string node_path, string Element, string name, string value)
            {
                try
                {

                    if (check_File_Path(xml_path) == 0)
                    {
                        //throw new Exception("XML FILE의 경로를 확인하세요.");
                        Console.WriteLine("XML FILE의 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        if (check_node(xml_path, Element, node_path) == 0)
                        {
                            //throw new Exception("중복된 Element가 존재합니다.");
                            Console.WriteLine("중복된 Element가 존재합니다.");
                            return 0;
                        }

                        else
                        {

                            if (node_path.Equals(""))                   //노드패스가 공백인 경우, 루트 밑에 붙인다.
                            {
                                XmlDocument Xml_Doc = new XmlDocument();
                                Xml_Doc.Load(xml_path);

                                XmlNode root;
                                //root = Xml_Doc.SelectSingleNode("root");              //수정 전
                                root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                                XmlNode newNode;
                                newNode = Xml_Doc.CreateElement(Element);

                                XmlAttribute newAttribute = Xml_Doc.CreateAttribute(name);
                                newAttribute.Value = value;

                                newNode.Attributes.Append(newAttribute);

                                root.AppendChild(newNode);

                                Xml_Doc.AppendChild(root);
                                Xml_Doc.Save(xml_path);
                                return 1;
                            }

                            else                                        //공백이 아닌 경우, 해당 경로 찾아가서 붙인다.
                            {
                                XmlDocument Xml_Doc = new XmlDocument();
                                Xml_Doc.Load(xml_path);

                                XmlNode root;
                                //root = Xml_Doc.SelectSingleNode("root");      //수정 전
                                root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                                //여기부터
                                XmlNode now_node;
                                now_node = root.SelectSingleNode(node_path);
                                //여기까지

                                XmlNode newNode;
                                newNode = Xml_Doc.CreateElement(Element);

                                XmlAttribute newAttribute = Xml_Doc.CreateAttribute(name);
                                newAttribute.Value = value;

                                newNode.Attributes.Append(newAttribute);

                                //root.AppendChild(newNode);
                                now_node.AppendChild(newNode);

                                Xml_Doc.AppendChild(root);
                                Xml_Doc.Save(xml_path);
                                return 1;
                            }

                        }
                    }

                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// <para>
            /// 자식 노드 생성(꼭 root 노드와 연결되진 않음)
            /// </para>
            /// <example>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// EXAMPLE--------------------------------------------------------------------------------------------------------------
            /// </para>
            /// <para>
            /// root에 Child_1, Child_2라는 자식노드가 있는 상태에서
            /// </para>
            /// <para>
            /// Child_1의 자식 노드를 생성하고 싶다면
            /// </para>
            /// <para>
            /// EX_1 -> AJ_XML_Add_Node("Child_1", "Grand_Child_1", "att_name", "att_value"); 이다
            /// </para>
            /// <para>
            /// 새로 생성한 Grand_Child_1노드 밑에 또 자식을 생성하고 싶다면
            /// </para>
            /// <para>
            /// EX_2 -> AJ_XML_Add_Node("Child_1/Grand_Child_1", "Super_Grand_Child_1", "att_name", "att_value");
            /// </para>
            /// <para>
            /// 으로 노드의 부모 경로를 추가하면, 계속 하위로 접근 가능
            /// </para>
            /// <para>
            /// -----------------------------------------------------------------------------------------------------------------------
            /// </para>
            /// </example>
            /// </summary>
            /// <param name="node_path">생성될 노드의 부모노드 경로</param>
            /// <param name="Element">생성될 노드의 Element</param>
            /// <param name="name">생성될 노드의 속성명</param>
            /// <param name="value">생성될 노드의 속성값</param>
            /// /// <returns>int형 리턴 -> 1 : 성공, 0 : 실패</returns>
            public static int AJ_XML_Add_Node(string node_path, string Element, string name, string value)
            {
                try
                {
                    if (check_File_Path(XML_File_Path) == 0)
                    {
                        //throw new Exception("XML FILE의 경로를 확인하세요.");
                        Console.WriteLine("XML FILE의 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        if (check_node(XML_File_Path, Element, node_path) == 0)
                        {
                            //throw new Exception("중복된 Element가 존재합니다.");
                            Console.WriteLine("중복된 Element가 존재합니다.");
                            return 0;
                        }

                        else
                        {

                            if (node_path.Equals(""))                   //노드패스가 공백인 경우, 루트 밑에 붙인다.
                            {
                                XmlDocument Xml_Doc = new XmlDocument();
                                Xml_Doc.Load(XML_File_Path);

                                XmlNode root;
                                //root = Xml_Doc.SelectSingleNode("root");              //수정 전
                                root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                                XmlNode newNode;
                                newNode = Xml_Doc.CreateElement(Element);

                                XmlAttribute newAttribute = Xml_Doc.CreateAttribute(name);
                                newAttribute.Value = value;

                                newNode.Attributes.Append(newAttribute);

                                root.AppendChild(newNode);

                                Xml_Doc.AppendChild(root);
                                Xml_Doc.Save(XML_File_Path);
                                return 1;
                            }

                            else                                        //공백이 아닌 경우, 해당 경로 찾아가서 붙인다.
                            {
                                XmlDocument Xml_Doc = new XmlDocument();
                                Xml_Doc.Load(XML_File_Path);

                                XmlNode root;
                                //root = Xml_Doc.SelectSingleNode("root");      //수정 전
                                root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                                //여기부터
                                XmlNode now_node;
                                now_node = root.SelectSingleNode(node_path);
                                //여기까지

                                XmlNode newNode;
                                newNode = Xml_Doc.CreateElement(Element);

                                XmlAttribute newAttribute = Xml_Doc.CreateAttribute(name);
                                newAttribute.Value = value;

                                newNode.Attributes.Append(newAttribute);

                                //root.AppendChild(newNode);
                                now_node.AppendChild(newNode);

                                Xml_Doc.AppendChild(root);
                                Xml_Doc.Save(XML_File_Path);
                                return 1;
                            }

                        }
                    }
                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// XML의 속성(Attribute) 추가
            /// <para>
            /// XML 파일의 구조는
            /// </para>
            /// <para>
            /// [Element명 Attribute_Name = "Attribute_Value"]
            /// </para>
            /// <example>
            /// <code>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// EXAMPLE-----------------------------------------------------------
            /// </para>
            /// <para>
            /// [김진남 나이 = "30"] 이라는 노드가 있다고 할 때
            /// </para>
            /// <para>
            /// AJ_XML_Add_Attribute("c:\\test.xml", "김진남", "회사", "AJ"); 를 하게 되면
            /// </para>
            /// <para>
            /// [김진남 나이 = "30" 회사 = "AJ"] 라고 Attribute가 추가 된다.
            /// </para>
            /// <para>
            /// root노드 밑의 자식 노드들의 자식 노드들은 Element에 "/"를 붙이는 식으로 접근 가능
            /// </para>
            /// <para>
            /// ex : AJ_XML_Add_Attribute("c:\\test.xml", "김진남/자식노드/자식의자식노드", "회사", "AJ")... 이런식
            /// </para>
            /// <para>
            /// -------------------------------------------------------------------
            /// </para>
            /// </code>
            /// </example>
            /// </summary>
            /// <param name="xml_path">XML File경로</param>
            /// <param name="element">Attribute를 추가할 Element명</param>
            /// <param name="add_attribute_name">추가될 Attribute_Name</param>
            /// <param name="add_attribute_value">추가될 Attribute_Value</param>
            /// <returns>int형 반환 -> 0: 실패, 1 : 성공</returns>
            public static int AJ_XML_Add_Attribute(string xml_path, string element, string add_attribute_name, string add_attribute_value)
            {
                try
                {
                    XmlDocument Xml_Doc = new XmlDocument();
                    Xml_Doc.Load(xml_path);

                    //XmlNode root = Xml_Doc.SelectNodes("root")[0];            //수정 전
                    XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                    XmlNode selected_node = root.SelectSingleNode(element);

                    XmlAttribute newAttribute = Xml_Doc.CreateAttribute(add_attribute_name);
                    newAttribute.Value = add_attribute_value;

                    selected_node.Attributes.Append(newAttribute);

                    Xml_Doc.Save(xml_path);
                    return 1;
                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }

            /// <summary>
            /// XML의 속성(Attribute) 추가
            /// <para>
            /// XML 파일의 구조는
            /// </para>
            /// <para>
            /// [Element명 Attribute_Name = "Attribute_Value"]
            /// </para>
            /// <example>
            /// <code>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// EXAMPLE-----------------------------------------------------------
            /// </para>
            /// <para>
            /// [김진남 나이 = "30"] 이라는 노드가 있다고 할 때
            /// </para>
            /// <para>
            /// AJ_XML_Add_Attribute("김진남", "회사", "AJ"); 를 하게 되면
            /// </para>
            /// <para>
            /// [김진남 나이 = "30" 회사 = "AJ"] 라고 Attribute가 추가 된다.
            /// </para>
            /// <para>
            /// root노드 밑의 자식 노드들의 자식 노드들은 Element에 "/"를 붙이는 식으로 접근 가능
            /// </para>
            /// <para>
            /// ex : AJ_XML_Add_Attribute("김진남/자식노드/자식의자식노드", "회사", "AJ")... 이런식
            /// </para>
            /// <para>
            /// -------------------------------------------------------------------
            /// </para>
            /// </code>
            /// </example>
            /// </summary>
            /// <param name="element">Attribute를 추가할 Element명</param>
            /// <param name="add_attribute_name">추가될 Attribute_Name</param>
            /// <param name="add_attribute_value">추가될 Attribute_Value</param>
            /// /// <returns>int형 반환 -> 0: 실패, 1 : 성공</returns>
            public static int AJ_XML_Add_Attribute(string element, string add_attribute_name, string add_attribute_value)
            {
                try
                {
                    XmlDocument Xml_Doc = new XmlDocument();
                    Xml_Doc.Load(XML_File_Path);

                    //XmlNode root = Xml_Doc.SelectNodes("root")[0];            //수정 전
                    XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                    XmlNode selected_node = root.SelectSingleNode(element);

                    XmlAttribute newAttribute = Xml_Doc.CreateAttribute(add_attribute_name);
                    newAttribute.Value = add_attribute_value;

                    selected_node.Attributes.Append(newAttribute);

                    Xml_Doc.Save(XML_File_Path);
                    return 1;
                }

                catch (Exception ex)
                {
                    return 0;
                }
                
            }
            #endregion

            #region XML_REMOVE

            /// <summary>
            /// <para>
            /// 해당 노드와 해당 노드의 하위 노드 삭제
            /// </para>
            /// <para>
            /// element에 "/"로 하위 노드 접근 가능
            /// </para>
            /// <para>
            /// EX) AJ_XML_Remove_Node("c:\\test.xml", "child/grad_child/);
            /// </para>
            /// </summary>
            /// <param name="xml_path">xml File 경로</param>
            /// <param name="element">타겟 Element명</param>
            /// <returns>int형 반환 -> 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Remove_Node(string xml_path, string element)
            {
                try
                {
                    if (check_File_Path(xml_path) == 0)
                    {
                        //throw new Exception("XML 파일 경로를 확인하세요.");
                        Console.WriteLine("XML 파일 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(xml_path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택

                        if (temp_node == null)
                        {
                            //throw new Exception("XML에 해당 Element가 존재하지 않습니다.");
                            Console.WriteLine("XML에 해당 Element가 존재하지 않습니다.");
                            return 0;
                        }
                        else
                        {
                            //root.RemoveChild(temp_node);
                            temp_node.ParentNode.RemoveChild(temp_node);            //루트의 자식이 아닐수도 있기 때문에
                            Xml_Doc.Save(xml_path);
                            return 1;
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("AJ_XML_Remove_Node 함수에서 에러");
                    return 0;
                }
                
            }

            /// <summary>
            /// <para>
            /// 해당 노드와 해당 노드의 하위 노드 삭제
            /// </para>
            /// <para>
            /// element에 "/"로 하위 노드 접근 가능
            /// </para>
            /// <para>
            /// EX) AJ_XML_Remove_Node("c:\\test.xml", "child/grad_child/);
            /// </para>
            /// <param name="element">타겟 Element명</param>
            /// <returns>int형 반환 -> 0 : 실패, 1 : 성공 </returns>
            public static int AJ_XML_Remove_Node(string element)
            {
                try
                {
                    if (check_File_Path(XML_File_Path) == 0)
                    {
                        //throw new Exception("XML 파일 경로를 확인하세요.");
                        Console.WriteLine("XML 파일 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(XML_File_Path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택

                        if (temp_node == null)
                        {
                            //throw new Exception("XML에 해당 Element가 존재하지 않습니다.");
                            Console.WriteLine("XML에 해당 Element가 존재하지 않습니다.");
                            return 0;
                        }
                        else
                        {
                            //root.RemoveChild(temp_node);
                            temp_node.ParentNode.RemoveChild(temp_node);            //루트의 자식이 아닐수도 있기때문에
                            Xml_Doc.Save(XML_File_Path);
                            return 1;
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("AJ_XML_Remove_Node 함수에서 에러");
                    return 0;
                }
                

            }


            /// <summary>
            /// 해당 Attribute(속성)삭제
            /// </summary>
            /// <param name="xml_path">xml File 경로</param>
            /// <param name="element">타겟 Element명</param>
            /// <param name="attribute_name">타겟 속성명</param>
            /// <returns>int형 반환 -> 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Remove_Attribute(string xml_path, string element, string attribute_name)
            {
                try
                {
                    if (check_File_Path(xml_path) == 0)
                    {
                        //throw new Exception("XML 파일 경로를 확인하세요.");
                        Console.WriteLine("XML 파일 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(xml_path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택

                        if (temp_node == null)
                        {
                            //throw new Exception("XML에 해당 Element가 존재하지 않습니다.");
                            Console.WriteLine("XML에 해당 Element가 존재하지 않습니다.");
                            return 0;
                        }
                        else
                        {
                            if (temp_node.Attributes.RemoveNamedItem(attribute_name) == null)
                            {
                                //throw new Exception("XML에 해당 Attribute가 존재하지 않습니다.");
                                Console.WriteLine("XML에 해당 Attribute가 존재하지 않습니다.");
                                return 0;
                            }
                            else
                            {
                                Xml_Doc.Save(xml_path);
                                return 1;
                            }

                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("AJ_XML_Remove_Attribute 함수에서 에러");
                    return 0;
                }
                
            }

            /// <summary>
            /// 해당 Attribute(속성)삭제
            /// </summary>
            /// <param name="element">타겟 Element명</param>
            /// <param name="attribute_name">타겟 속성명</param>
            /// <returns>int형 반환 -> 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Remove_Attribute(string element, string attribute_name)
            {
                try
                {
                    if (check_File_Path(XML_File_Path) == 0)
                    {
                        //throw new Exception("XML 파일 경로를 확인하세요.");
                        Console.WriteLine("XML 파일 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(XML_File_Path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택


                        if (temp_node == null)
                        {
                            //throw new Exception("XML에 해당 Element가 존재하지 않습니다.");
                            Console.WriteLine("XML에 해당 Element가 존재하지 않습니다.");
                            return 0;
                        }
                        else
                        {
                            if (temp_node.Attributes.RemoveNamedItem(attribute_name) == null)
                            {
                                //throw new Exception("XML에 해당 Attribute가 존재하지 않습니다.");
                                Console.WriteLine("XML에 해당 Attribute가 존재하지 않습니다.");
                                return 0;
                            }
                            else
                            {
                                Xml_Doc.Save(XML_File_Path);
                                return 1;
                            }

                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("AJ_XML_Remove_Attribute 함수에서 에러");
                    return 0;
                }
                
            }
            #endregion

            #region XML_MODIFY
            //노드의 속성값(Attribute Value) 변경.

            /// <summary>
            /// 해당 노드의 속성값(Attribute Value)을 변경한다.
            /// </summary>
            /// <param name="xml_path">XML File 경로</param>
            /// <param name="element">타겟 Element명</param>
            /// <param name="attribute_name">타겟 Attribute Name</param>
            /// <param name="new_value">새로 입력될 Attribute Value</param>
            /// <returns>int형 리턴 -> 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Modify_Attribute_Value(string xml_path, string element, string attribute_name, string new_value)
            {
                try
                {
                    if (check_File_Path(xml_path) == 0)
                    {
                        //throw new Exception("XML 파일 경로를 확인하세요.");
                        Console.WriteLine("XML 파일 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(xml_path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택

                        if (temp_node == null)
                        {
                            //throw new Exception("XML에 해당 Element가 존재하지 않습니다.");
                            Console.WriteLine("XML에 해당 Element가 존재하지 않습니다.");
                            return 0;
                        }
                        else
                        {
                            XmlAttribute temp_attr;
                            temp_attr = temp_node.Attributes[attribute_name];

                            temp_attr.Value = new_value;

                            //selected_node.Attributes.Append(newAttribute);
                            temp_node.Attributes.InsertAfter(temp_attr, temp_node.Attributes[attribute_name]);

                            AJ_XML_Remove_Attribute(xml_path, element, attribute_name);

                            Xml_Doc.Save(xml_path);
                            return 1;
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("AJ_XML_Modify_Attribute_Value 함수에서 에러");
                    return 0;
                }
                
            }


            /// <summary>
            /// 해당 노드의 속성값(Attribute Value)을 변경한다.
            /// </summary>
            /// <param name="element">타겟 Element명</param>
            /// <param name="attribute_name">타겟 Attribute Name</param>
            /// <param name="new_value">새로 입력될 Attribute Value</param>
            /// <returns>int형 리턴 -> 0 : 실패, 1 : 성공</returns>
            public static int AJ_XML_Modify_Attribute_Value(string element, string attribute_name, string new_value)
            {
                try
                {
                    if (check_File_Path(XML_File_Path) == 0)
                    {
                        //throw new Exception("XML 파일 경로를 확인하세요.");
                        Console.WriteLine("XML 파일 경로를 확인하세요.");
                        return 0;
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(XML_File_Path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택

                        if (temp_node == null)
                        {
                            //throw new Exception("XML에 해당 Element가 존재하지 않습니다.");
                            Console.WriteLine("XML에 해당 Element가 존재하지 않습니다.");
                            return 0;
                        }
                        else
                        {

                            XmlAttribute temp_attr;
                            temp_attr = temp_node.Attributes[attribute_name];

                            temp_attr.Value = new_value;

                            //selected_node.Attributes.Append(newAttribute);
                            temp_node.Attributes.InsertAfter(temp_attr, temp_node.Attributes[attribute_name]);

                            AJ_XML_Remove_Attribute(XML_File_Path, element, attribute_name);



                            //selected_node.Attributes.Append(newAttribute);
                            //temp_node.Attributes.GetNamedItem(
                            Xml_Doc.Save(XML_File_Path);
                            return 1;
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("AJ_XML_Modify_Attribute_Value 함수에서 에러");
                    return 0;
                }
                
            }
            #endregion

            #region XML_READ
            //XML 리드 all
            /// <summary>
            /// <para>
            /// 해당 경로의 XML에서 속성 값을 읽어 string으로 반환
            /// </para>
            /// <example>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// Example----------------------------------------------
            /// </para>
            /// <para>
            /// XML DATA예시
            /// </para>
            /// <para>
            /// 　-[root]
            /// </para>
            /// <para>
            /// 　　-[계정 ID="jinnam"]
            /// </para>
            /// <para>
            /// 　　　[부계정 ID="jinnam_2"/]
            /// </para>
            /// <para>
            /// 　　[/계정]
            /// </para>
            /// <para>
            /// 　　[계정2 ID = "AJ"/]
            /// </para>
            /// <para>
            /// 　[/root]
            /// </para>
            /// <para>
            /// 일때
            /// <para>
            /// AJ_XML_READ(XML_PATH, "계정", "ID") -> "jinnam" 반환
            /// </para>
            /// </para>
            /// <para>
            /// AJ_XML_READ(XML_PATH, "계정2", "ID") -> "AJ" 반환
            /// </para>
            /// <para>
            /// AJ_XML_READ(XML_PATH, "계정/부계정", "ID") -> "jinnam_2" 반환
            /// </para>
            /// <code>
            /// </code>
            /// <para>
            /// ------------------------------------------------------
            /// </para>
            /// </example>
            /// </summary>
            /// <param name="xml_path">XML 경로</param>
            /// <param name="element">요소</param>
            /// <param name="attribute_name">속성 이름</param>
            /// <returns>String return, 동작 실패 시, [ERROR : Message] 반환</returns>
            public static string AJ_XML_Read(string xml_path, string element, string attribute_name)
            {
                try
                {
                    if (check_File_Path(xml_path) == 0)
                    {
                        //throw new Exception("XML FILE의 경로를 확인하세요.");
                        return "[ERROR : XML FILE의 경로를 확인하세요.";
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(xml_path);                                            //경로의 XML 로드

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];                   //루트노드의 자식 노드들 중, 입력된 element의 노드 선택

                        string result = "";

                        for (int i = 0; i < root.SelectNodes(element).Count; i++)           //음..이 부분은 바꿀 수 있는 다른 방법이 있나
                        {
                            //aa = qqq.Attributes["ID"].Value;
                            result = temp_node.Attributes[attribute_name].Value;            //선택된 노드에서 속성이름으로 검색, 속성값 반환
                        }

                        return result;
                    }
                }

                catch (Exception ex)
                {
                    return "[ERROR : AJ_XML_Read 함수에서 에러]";
                }
                
            }

            /// <summary>
            /// <para>
            /// AJ_XML_Set_Path에서 설정한 경로의 XML에서 속성 값을 읽어 string으로 반환
            /// </para>
            /// <example>
            /// <para>
            /// 　
            /// </para>
            /// <para>
            /// Example----------------------------------------------
            /// </para>
            /// <para>
            /// XML DATA예시
            /// </para>
            /// <para>
            /// 　-[root]
            /// </para>
            /// <para>
            /// 　　-[계정 ID="jinnam"]
            /// </para>
            /// <para>
            /// 　　　[부계정 ID="jinnam_2"/]
            /// </para>
            /// <para>
            /// 　　[/계정]
            /// </para>
            /// <para>
            /// 　　[계정2 ID = "AJ"/]
            /// </para>
            /// <para>
            /// 　[/root]
            /// </para>
            /// <para>
            /// 일때
            /// <para>
            /// AJ_XML_READ("계정", "ID") -> "jinnam" 반환
            /// </para>
            /// </para>
            /// <para>
            /// AJ_XML_READ("계정2", "ID") -> "AJ" 반환
            /// </para>
            /// <para>
            /// AJ_XML_READ("계정/부계정", "ID") -> "jinnam_2" 반환
            /// </para>
            /// <code>
            /// </code>
            /// <para>
            /// ------------------------------------------------------
            /// </para>
            /// </example>
            /// </summary>
            /// <param name="element">요소</param>
            /// <param name="attribute_name">속성 이름</param>
            /// <returns>String return, 동작 실패 시, [ERROR : Message] 반환</returns>
            public static string AJ_XML_Read(string element, string attribute_name)
            {
                try
                {
                    if (check_File_Path(XML_File_Path) == 0)
                    {
                        //throw new Exception("XML FILE의 경로를 확인하세요.");
                        return "[ERROR : XML FILE의 경로를 확인하세요]";
                    }

                    else
                    {
                        XmlDocument Xml_Doc = new XmlDocument();
                        Xml_Doc.Load(XML_File_Path);

                        //XmlNode root = Xml_Doc.SelectNodes("root")[0];        //수정 전
                        XmlNode root = Xml_Doc.FirstChild.NextSibling;          //수정 후, XML문서의 FirstChild는 <?XML version="1.0"....> 그 바로 밑은 루트

                        XmlNode temp_node = root.SelectNodes(element)[0];

                        string result = "";

                        for (int i = 0; i < root.SelectNodes(element).Count; i++)
                        {
                            //aa = qqq.Attributes["ID"].Value;
                            result = temp_node.Attributes[attribute_name].Value;
                        }

                        return result;
                    }
                }

                catch (Exception ex)
                {
                    return "[ERROR : AJ_XML_Read 함수에서 에러]";
                }
                

            }
            #endregion



        }
    }
}
