alter table bookings_template_metadata add column is_fortnightly BOOLEAN default 0 not null;
alter table booking_templates add column is_first_week BOOLEAN default 0 not null;
