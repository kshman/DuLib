namespace Du;

/// <summary>
/// 상수 프로시져 리퍼런스
/// Constant & Procedure & Reference
/// </summary>
internal class Cpr
{
    // 단문
    internal const string reconn = "재연결";
    internal const string conn = "연결";
    internal const string count = "개수";
    internal const string address = "주소";
    internal const string modbus_client = "ModBus 클라이언트";
    internal const string coil = "코일";
    internal const string discrete = "디스크릿";
    internal const string holding = "홀딩";
    internal const string input = "인풋";
    internal const string connected = "연결되어 있어요";
    internal const string unkversion = "알 수 없는 버전";

	// 임무	
	internal const string task_read_coil = "코일 읽기";
	internal const string task_read_discrete_input = "디스크릿 입력 읽기";
    internal const string task_read_hold_regi = "홀딩 레지스터 읽기";
    internal const string task_read_in_regi = "입력 레지스터 읽기";
    internal const string task_read_devinfo_raw = "디바이스 정보 RAW 읽기";
    internal const string task_write_coil = "코일 쓰기";
    internal const string task_write_regi = "레지스터 쓰기";

    // 예외
    internal const string ex_argument = "인수를 확인 하세요";
    internal const string ex_enter_read = "읽기 잠금에 들어가려 했네요";
    internal const string ex_enter_upgradable_read = "업그레드 읽기 잠금에 들어가려 했네요";
    internal const string ex_enter_write = "쓰기 잠금에 들어가려 했네요";
    internal const string ex_no_more_by_eof_stream = "바이트를 더 읽을 수 있을거 같은데, 스트림이 끝났어요";
    internal const string ex_not_match_with = "매치가 안되요: ";
    internal const string ex_invalid_register_type = "잘못된 레지스터 형식이예요";
    internal const string ex_invalid_coil_type = "잘못된 코일 형식이예요";
    internal const string ex_invalid_input_type = "잘못된 인풋 형식이예요";
    internal const string ex_no_address_gap_within_request = "요청에는 주소 차이가 허용되지 않아요";
    internal const string ex_no_host_found = "호스트를 찾을 수 없어요";
    internal const string ex_client_no_conn = "클라이언트가 연결되어 있지 않아요";
    internal const string ex_client_cant_open_stream = "클라이언트가 스트림을 열 수 없어요";
    internal const string ex_invalid_protocol_identifier = "프로토콜 식별자가 틀렸어요";
    internal const string ex_data_length_less = "데이터 길이가 짧아요";
    internal const string ex_data_length_many = "데이터 길이가 길어요";
    internal const string ex_unk_mei_with = "알 수 없는 MEI 타입: ";
    internal const string ex_unk_func_with = "알 수 없는 펑션 코드: ";
    internal const string ex_no_data = "데이터가 없어요";
    internal const string ex_no_payload = "페이로드가 없어요";
    internal const string ex_need_reg_at_least = "레지스터는 적어도 다음 이상은 필요해요: ";
    internal const string ex_no_device_exist = "장치가 없어요";

	// logger용 메시지
	internal const string log_method_enter = "{method} 들어왔어요";
    internal const string log_method_leave = "{method} 나가요";
    internal const string log_now_starting = "{target} 지금 시작해요";
    internal const string log_started = "{target} 시작했어요";
    internal const string log_now_stopping = "{target} 지금 멈춰요";
    internal const string log_stopped = "{target} 멈췄어요";
    internal const string log_windows_only = "윈도우 전용 기능이예요";
    internal const string log_starting_conn = "{conn}을 지금 시작해요";
    internal const string log_conn_success_with = "성공적으로 {task}했어요";
    internal const string log_conn_task_ex = "'{task}' 실패: {ex}, 다시 연결할께요";
    internal const string log_conn_fail_ex = "'{task}' 실패: {ex}";
    internal const string log_conn_timeout = "{task}이 시간초과 했어요: {timeout}초";
    internal const string log_conn_try_again = "{task} 실패: {ex}, 다시 시도해보세요";
    internal const string log_start_recv = "응답 받기를 시작했어요";
    internal const string log_dev_rd_coil_cnt = "#{deviceId} 장치에서 {startAddress}부터 {count}개의 코일을 읽어요";
    internal const string log_dev_rd_discrete_input_cnt = "#{deviceId} 장치에서 {startAddress}부터 {count}개의 디스크릿 입력을 읽어요";
    internal const string log_dev_rd_hold_regi_cnt = "#{deviceId} 장치에서 {startAddress}부터 {count}개의 홀딩 레지스터를 읽어요";
    internal const string log_dev_rd_in_regi_cnt = "#{deviceId} 장치에서 {startAddress}부터 {count}개의 입력 레지스터를 읽어요";
    internal const string log_dev_rd_devinfo_raw = "#{deviceId} 장치에서 {categoryId}의 {objectId} 정보를 읽어요";
    internal const string log_dev_wr_coil = "#{deviceId} 장치에 코일을 써요: {coil}";
    internal const string log_dev_wr_regi = "#{deviceId} 장치에 레지스터를 써요: {register}";
    internal const string log_dev_wr_coil_cnt = "#{deviceId} 장치에 {first}부터 {count}개의 코일을 써요";
    internal const string log_dev_wr_regi_cnt = "#{deviceId} 장치에 {first}부터 {count}개의 레지스터를 써요";
    internal const string log_tr_no_match = "#{transactionId} 트랜젝션의 응답을 받았어요. 하지만 매칭 요청을 확인할 수가 없어요";
    internal const string log_tr_resp = "#{transactionId} 트랜젝션의 응답을 받았어요";
    internal const string log_recv_invalid_data = "잘못된 데이터를 받았어요: {ex}";
    internal const string log_recv_error_ex = "받다가 오류({name})가 났어요: {ex}";
    internal const string log_stop_resp = "응답 받기를 멈췄어요";
    internal const string log_tr_add_recv_que = "#{transactionId} 트랜젝션을 받기 큐에 추가했어요";
    internal const string log_sending_req = "요청을 보내고 있어요: {request}";
    internal const string log_tr_send_req = "#{transactionId} 트랜젝션의 요청을 보냈어요";
    internal const string log_send_error_ex = "보내다가 오류({name})가 났어요: {ex}";
    internal const string log_tcp_server_stared = "모드버스 서버를 시작했어요: {ListenAddress}:{Port}/tcp에서 리슨 중이예요";
    internal const string log_tcp_client_connected = "클라이언트가 연결됐어요: {Address}";
    internal const string log_tcp_client_disconnected = "클라이언트 연결이 끊겼어요: {Address}";
    internal const string log_tcp_recv_invalid_data = "{Address}에서 잘못된 데이터를 받았어요: {ex}";
    internal const string log_unexcepted_error_on = "{Name}에 예상치 못한 오류가 났어요: {ex}";

    // 판단형
    internal static string GetConnDesc(bool wasConn) => wasConn ? reconn : conn;
}
