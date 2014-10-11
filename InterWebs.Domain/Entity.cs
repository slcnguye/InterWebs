using System;

namespace InterWebs.Domain
{
    public abstract class Entity
    {
        private int? requestedHashCode;
        private Guid id;

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public virtual Guid Id
        {
            get { return id; }
            set
            {
                id = value;
                AfterIdSet();
            }
        }

        protected virtual void AfterIdSet()
        {
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, null) ? Equals(right, null) : left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public void ChangeCurrentIdentity(Guid identity)
        {
            if (identity != Guid.Empty)
            {
                Id = identity;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var item = (Entity)obj;

            return item.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!requestedHashCode.HasValue)
            {
                // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                requestedHashCode = Id.GetHashCode() ^ 31;
            }

            return requestedHashCode.Value;
        }
    }
}
