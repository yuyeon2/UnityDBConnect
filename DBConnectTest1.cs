using MySql.Data;     
using UnityEngine;
using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class DBConnectTest1 : MonoBehaviour {

    //200210 3d 오브젝트 raycast 클릭 시 해당 구역 정보 값 표출을 위한 데이터베이스 연동 스크립트

    //230314 기존 스크립트 수정 후 깃허브 저장이라 별도 테스트 필요

    MysqlConnectection sqlConnect = null;
    string ip = "ip";
    string port = "port";
    string database = "DBname";
    string userID = "userID";
    string userPW = "1234555";

    public static DataLoadMgr dataLoad;

    void Start()
    {
        dataLoad = GameObject.Find("SceneScript").GetComponent<DataLoadMgr>();
    }

    public void ConnectSQL()
    {
        string sqlDataBase = "Server=" + ip + ";Port=" + port + ";Database=" + database + ";UserId=" + userID + ";Password=" + userPW + "";

        try
        {
            sqlConnect = new MysqlConnectection(sqlDataBase);
            sqlConnect.Open();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void DisConnectSQL()
    {
        sqlConnect.Close();
    }

    public void CommandSQL(string commandAll) 
    {
        ConnectSQL();

        MySqlCommand sqlCommand = new MySqlCommand(commandAll, sqlConnect);
        sqlCommand.ExecuteNonQuery();

        DisConnectSQL();
    }

    public DataTable DataTableSQL(string sqlCommand) 
    {
        DataTable dataTable = new DataTable(); 

        sqlConnectect();

        MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCommand, sqlConnect);

        adapter.Fill(dataTable); 

        DisConnectSQL();

        return dataTable; 
    }

    DateTime datetime;

    //날짜 조회 후 화면의 마커 클릭 시 데이터베이스 값 호출
    public void Marker_click(string date, string code)
    {
        //날짜 형식이 맞지 않을 경우 이 외의 예외처리는 다른 스크립트에 있음
        //문구는 임의로 삽입
        if (date.Length != 12)
        {
            dt_m.warning_popup.SetActive(true);
            dt_m.warning_popup_txt.GetComponent<Text>().text = "No matching data found.";
            dt_m.StartCoroutine("Blink", dt_m.warning_popup_txt);
        }
        else
        {
            //기준 날짜가 UST라서 한국 표준 시간으로 맞추기 위한 부분
            dt = DateTime.ParseExact(date, "yyyyMMddHHmm", null).AddHours(-9);
            date = dt.ToString("yyyy") + "-" + dt.ToString("MM") + "-" + dt.ToString("dd") + " "
                + dt.ToString("HH") + ":" + dt.ToString("mm");

            string query = @"
            SELECT A_TABLE.ADMIN, B_TABLE.NAME, B_TABLE.VALUE 
            FROM A_TABLE, B_TABLE
            WHERE A_TABLE.DATE =""" + date + "\"" +
            @"and (A_TABLE.INDEX_CODE =""" + code + "\"" +
            @"and B_TABLE.ADMIN_CODE = """
            + code + "\"" + ");";

            Data_Connect(query);
        }
    }

    public Text[] txtTable; 

    public void Data_Connect(string query)
    {
        string Connect = "Server=" + ip + ";Port=" + port + ";Database=" + database + ";UserId=" + userID + ";Password=" + userPW + "";
        
        MysqlConnectection conn = new MysqlConnectection(strConn);
        conn.Open();

        MySqlCommand command = new MySqlCommand(query, conn);
        MySqlDataReader dataReader = command.ExecuteReader();

        dataReader.Read();

        txtTable[0].text = dataReader[0].ToString();

        //호출 된 데이터 정보 ui text에 뿌리기
        for (int i = 1; i < dataReader.FieldCount; i++)
        {
            if (Convert.ToDouble(dataReader[1]) == -1e+30)
            {
                txtTable[i].text = "0.0";
            }
            else
            {
                txtTable[i].text = Convert.ToSingle(dataReader[i]).ToString("N1");
            }
        }

        conn.Close(); 
    }

}
