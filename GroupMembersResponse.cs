namespace fb_groups_intersector
{
    public class GroupMembersResponse
    {
        public GroupMember[] data { get; set; }
        public Paging paging { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
        public string next { get; set; }
    }

    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class GroupMember
    {
        public string name { get; set; }
        public string id { get; set; }
        public bool administrator { get; set; }
    }
}