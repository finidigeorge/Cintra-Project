alter table booking_templates add payment_type_id INTEGER REFERENCES payment_types(id);
alter table booking_templates add payment_options varchar(200);


begin transaction; 

create temporary table temp_table(template_metadata_id integer, payment_type_id integer);
insert into temp_table
select t.template_metadata_id, max(payment_type_id) as payment_type_id from 
(
select b.template_metadata_id, bp.payment_type_id
from 
bookings b, booking_payments bp 
where bp.booking_id = b.id and b.is_deleted = 0 and bp.is_deleted = 0 and bp.payment_type_id is not null
) t
where t.template_metadata_id is not null
group by t.template_metadata_id;

update booking_templates
set payment_type_id = (select payment_type_id from temp_table tt where tt.template_metadata_id = booking_templates.template_metadata_id)
where exists (select 1 from temp_table tt where tt.template_metadata_id = booking_templates.template_metadata_id);

drop table temp_table;

commit transaction; 

