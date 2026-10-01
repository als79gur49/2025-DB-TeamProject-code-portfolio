public interface IState
{
    //시작 시 1회 호출
    public void Enter(AIInput input);

    //상태 업데이트 할 때 매 프레임마다 호출
    public void Execute(AIInput input);

    //상태 종료할 때 1회 호출
    public void Exit(AIInput input);

}
