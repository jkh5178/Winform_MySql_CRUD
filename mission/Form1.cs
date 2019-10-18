using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace mission
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;// DB서버에 연결하기 위한 클래스
        private MySqlDataAdapter mySqlDataAdapter;//DB서버의 데이터를 받아오기 위한 클래스
        private DataSet Data = new DataSet();//data를 저장할 DataSet 클래스
        public Form1()//폼 생성자
        {
            InitializeComponent();//윈폼을 초기화
            connection = new MySqlConnection("Server=localhost;Database=student;Uid=(디비계정);Pwd=(비밀번호);");//서버의 주소와 DB, ID, Pw입력
        }
        private void Form1_Load(object sender, EventArgs e)// 폼이 불어올때 실행
        {
            mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM student;", connection);//현재 DB의 테이블 정보로드
            mySqlDataAdapter.Fill(Data, "Databases");
            dataGridView1.DataSource = Data.Tables["Databases"];//DB를 불러온후
            Data.Tables["Databases"].Rows.Clear();//제목행 이외의 정보 삭제
        }
        //테이블 읽기
        private void btn_read_Click(object sender, EventArgs e)//read버튼 클릭시
        {
            mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM student;", connection);//현재 연결된 DB에서 학생정보 전부 불러오기
            mySqlDataAdapter.Fill(Data, "Databases");
            dataGridView1.DataSource = Data.Tables["Databases"];//데이터그리드에 DATASET클래스 띄우기
        }
        
        
        //데이터 제거
        private void btn_remove_Click(object sender, EventArgs e)//데이터그리드에 삭제를 원하는 행 선택 후 remove버튼을 눌렀을때
        {
            connection.Open();//DB와 연결을 연다.
            MySqlCommand command = new MySqlCommand("DELETE  FROM student WHERE no=" + dataGridView1.CurrentRow.Cells[2].Value.ToString(), connection);
            //현제행의 학생번호를 사용하여 DB remove 쿼리 전송
            command.ExecuteNonQuery();//쿼리 실행
            connection.Close();//디비 종료
            Data.Tables["Databases"].Rows.RemoveAt(dataGridView1.CurrentRow.Index);//데이터그리드에도 삭제된 행 제거
        }
        
        
        //데이터 추가
        private void btn_create_Click(object sender, EventArgs e)
            //원하는 데이터를 입력후 create버튼을 누를시
        {
            connection.Open();
            DataTable dtChanged = Data.Tables["Databases"].GetChanges(DataRowState.Added);
            //현제 dataset중 테이블중에서 줄이 추가된 정보를 받음
            foreach (DataRow row in dtChanged.Rows)
            {//현재 상태가 add인 테이블중 추가된 데이터행 찾기
                if (row.RowState == DataRowState.Added)
                {//행의 상태가 추가인경우
                    MySqlCommand command = new MySqlCommand("insert into student values(" + row[0]+","+ row[1] + "," + row[2] + ",'" + row[3] + "','" + row[4] + "');", connection);
                    //추가된 행의 정보 DB로 추가
                    command.ExecuteNonQuery();
                }
                
            }
            connection.Close();
        }

        //데이터 변경
        private void update_Click(object sender, EventArgs e)
        {//데이터 수정하여 db에 업로드시
            connection.Open();
            DataTable dtChanged = Data.Tables["Databases"].GetChanges(DataRowState.Modified);
            //현재 테이블에 수정된 부분이 있을 경우 해당 태이블 복사
            foreach (DataRow row in dtChanged.Rows)
            {//각 행별 변경된 부분이 있나 확인
                if (row.RowState == DataRowState.Modified)
                {//변경된 부분이 있을시
                    //변경전과 변경후의 값을 저장할 변수 선언
                    object beforevalue = null;
                    object aftervalue = null;
                    
                    foreach (DataColumn col in dtChanged.Columns)
                    {//변경된 행의 열별로 변경된 부분 검사
                        //변경 값과 현재값을 저장한다.
                        beforevalue = row[col, DataRowVersion.Original];
                        aftervalue = row[col, DataRowVersion.Current];
                        //현재값을 변경값으로 바꾸는 쿼리를 준비
                        MySqlCommand command = new MySqlCommand("update student set "+col.ToString()+"= '"+aftervalue.ToString()+"'where no="+row[2].ToString(), connection);
                        command.ExecuteNonQuery();//DB에 업로드
                    }
                }
            }
             connection.Close();
        }
    }
}
